Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsdevoluciones
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Devolver(ByVal proveedor As Integer, ByVal usuario As String, ByVal serie As String, ByVal numero As String, ByVal total As Double, ByVal listproductos As List(Of productos)) As String

        Dim result As String = ""
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion



        Try
            Dim sql As String = "INSERT INTO [dbo].[ENC_DEV_COMPRA]([Usuario],[id_empresa],[Serie_Fact],[Num_Fact],[Fecha],[Total_Factura],[Id_Proveedor],[estatus]) " &
            "VALUES ('" & usuario & "',(select top (1) id_empresa from USUARIO where USUARIO = '" & usuario & "'),'" & serie & "','" & numero & "', getdate(), " & total & "," & proveedor & ",1)"


            'ejecuto primer comando sql
            comando.CommandText = sql
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()

            For Each item As productos In listproductos

                'INSERTA LOS DATOS 
                Dim sql2 As String = "INSERT INTO [dbo].[DET_DEV_COMPRA]([id_enc],[Cantidad_Articulo],[Precio_Unit_Articulo],[Sub_Total],[Descuento],[Id_Art],[Id_Bod]) " &
                    "VALUES (" & id & "," & item.cantidad & "," & item.costo & "," & (item.costo * item.cantidad) & ",0.00," & item.id & ", " & item.bodega & "); "

                Dim existencia_actual As Integer = ObtenerCantidadProducto(item.id, item.bodega)

                sql2 = sql2 & " UPDATE [dbo].[Existencias] SET [Existencia_Deta_Art] = " & existencia_actual - item.cantidad & " WHERE Id_Bod = " & item.bodega & " and Id_Art = " & item.id

                comando.CommandText = sql2
                comando.ExecuteNonQuery()

            Next
            transaccion.Commit()
            result = "SUCCESS|DEVOLCION A PROVEEDORES GENERADA EXITOSAMENTE|"
        Catch ex As Exception
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try


        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer, ByVal idbodega As Integer) As Integer
        Dim SQL As String = "Select Existencia_Deta_Art As cantidad from  Existencias where Id_Art = " & idart & " And id_bod = " & idbodega

        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("cantidad")
            Next
        Next

        Return result

    End Function

    Public Class productos
        Public id As Integer
        Public cantidad As Integer
        Public costo As Double
        Public codigo As String
        Public descripcion As String
        Public arancel As Integer
        Public bodega As Integer
    End Class
End Class