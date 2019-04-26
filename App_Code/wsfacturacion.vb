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
Public Class wsfacturacion
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Facturar(ByVal usuario As String, ByVal total As Double, ByVal descuento As Double, ByVal idcliente As Integer, ByVal diascredito As Integer, ByVal listproductos As List(Of productos), ByVal listpagos As List(Of pagos)) As String
        Dim result As String

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

            Dim serie As String = "FAC"
            Dim eliminaDoc1 As String = "FACE66"
            Dim eliminaDoc2 As String = "FACE63"
            Dim eliminaDoc3 As String = serie & "001"
            Dim retornoData As String() = Factura_ElectronicaDemo(serie).Split("|")
            Dim firma As String = retornoData(0)
            Dim cae = retornoData(1)

            Dim totalsiniva As Double = total / 1.12
            Dim iva As Double = totalsiniva * 0.12

            Dim empresa As String = "SELECT id_empresa  FROM [ERPDEVLYNGT].[dbo].[USUARIO] where USUARIO = '" & usuario & "'"
            Dim sucursal As String = "SELECT id_sucursal  FROM [ERPDEVLYNGT].[dbo].[USUARIO] where USUARIO = '" & usuario & "'"

            'INSERCION DE LA FACTURA
            Dim str1 As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[ENC_FACTURA]([USUARIO],[id_empresa],[Serie_Fact],[Fecha],[firma],[Cae],[Total_Factura],[Iva_Factura],[Total_sin_iva],[Total_Descuento],[Id_Clt],[dias_cred],[id_suc]) " &
                "VALUES('" & usuario & "', (" & empresa & "),'" & serie & "',GETDATE(),'" & firma & "','" & cae & "'," & total & "," & Math.Round(iva, 2) & "," & Math.Round(totalsiniva, 2) & "," & descuento & "," & idcliente & "," & diascredito & ", (" & sucursal & ") )"

            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()


            'INSERTAMOS RECIVO
            Dim strRecivo As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[ENC_RECIBO] ([fecha],[Id_Clt],[Usuario],[empresa],[sucursal],[estado]) " &
                " VALUES(GETDATE()," & idcliente & ",'" & usuario & "',(" & empresa & "),(" & sucursal & "),1);"

            comando.CommandText = strRecivo
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE RECIVO
            comando.CommandText = "SELECT @@IDENTITY"
            Dim idRecivo As Integer = comando.ExecuteScalar()


            'INSERTAMOS EL RECIVO EN LA FACTURA
            Dim strFac_RECIVO As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[DET_RECIBO_FACT]([idRecibo],[id_enc],[abonado]) " &
                "VALUES(" & idRecivo & "," & id & "," & total & ");"

            comando.CommandText = strFac_RECIVO
            comando.ExecuteNonQuery()

            'INSERCION DEL DETALLE DE LA FACTURA
            For Each item As productos In listproductos

                Dim totalsinivaDesc As Double = (item.cantidad * item.precio) / 1.12
                Dim ivadesc As Double = totalsinivaDesc * 0.12

                Dim str2 As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[DET_FACTURA] " &
                "([id_enc],[Cantidad_Articulo],[Precio_Unit_Articulo],[Sub_Total],[Descuento],[Iva],[Total_Sin_Iva],[Total],[Id_Art],[costoPromedio],[Id_Bod])" &
                "VALUES(" & id & "," & item.cantidad & "," & item.precio & "," & (item.cantidad * item.precio) & ",0.00," & Math.Round(ivadesc, 2) & "," & Math.Round(totalsinivaDesc, 2) & "," & Math.Round((item.cantidad * item.precio), 2) & "," & item.id & ", ROUND((select CONVERT(varchar,costo_art) from [ERPDEVLYNGT].[dbo].[Articulo] where id_art = " & item.id & "),2)," & item.bodega & ");"

                Dim existencia_actual As Integer = ObtenerCantidadProducto(item.id, item.bodega)

                str2 = str2 + " UPDATE [dbo].[Existencias] SET [Existencia_Deta_Art] = " & existencia_actual - item.cantidad & " WHERE Id_Bod = " & item.bodega & " and Id_Art = " & item.id

                comando.CommandText = str2
                comando.ExecuteNonQuery()
            Next


            'INSERCION DEL DETALLE DEL RECIVO (METODOS DE PAGO)

            For Each item As pagos In listpagos
                Dim StrPago As String = "INSERT INTO [dbo].[DET_RECIBO]([idRecibo],[tipoPago],[documento],[valor]) " &
                    "VALUES(" & idRecivo & ",'" & item.tipo & "','" & item.informacion & "'," & item.valor - item.cambio & ")"
                comando.CommandText = StrPago
                comando.ExecuteNonQuery()
            Next


            transaccion.Commit()


            result = "SUCCESS| DATOS FACTURADOS EXITOSAMENTE"


        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer, ByVal idbodega As Integer) As Integer
        Dim SQL As String = "Select Existencia_Deta_Art as cantidad from ERPDEVLYNGT.dbo.Existencias where Id_Art = " & idart & " and id_bod = " & idbodega

        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("cantidad")
            Next
        Next

        Return result

    End Function


    Public Function Factura_ElectronicaDemo(ByVal serie As String) As String

        Dim chars As String = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789#/="
        Dim unit As String = ""
        Dim r As New Random
        Dim fechaHora As DateTime = DateTime.Now

        For i As Integer = 1 To 64

            Dim siguiente As Integer = r.Next(0, chars.Length)
            unit &= chars.Substring(siguiente, 1)

        Next

        Dim siguienteTurno As String = ObtenerSiguienteCorrelativo(serie)

        Dim firma As String = "FACE63" & serie & siguienteTurno
        Dim cae As String = unit

        Return firma & "|" & cae

    End Function


    <WebMethod()>
    Public Function ObtenerSiguienteCorrelativo(ByVal serie As String) As String
        Dim SQL As String = "SELECT ISNULL(MAX(CAST(SUBSTRING(firma, 10, 25) as numeric)), 180000000000) + 1 as Siguiente FROM [ERPDEVLYNGT].[dbo].[ENC_FACTURA] WHERE Serie_Fact = '" & serie & "';"

        Dim result As String = ""
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            result = TablaEncabezado.Rows(i).Item("Siguiente").ToString
        Next

        Return result

    End Function


    Public Class productos
        Public id As Integer
        Public cantidad As Integer
        Public bo As String
        Public bodega As Integer
        Public precio As Double

    End Class


    Public Class pagos
        Public tipo As Integer
        Public valor As Double
        Public informacion As String
        Public tipoPagoText As String
        Public cambio As Double
    End Class


End Class