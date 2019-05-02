Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class wstraslados
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function ObtenerExistenciasPorBodega(ByVal bodega As Integer) As IList(Of productos)

        Dim sql As String = "  SELECT a.id_art, e.Existencia_Deta_Art, a.Des_Art, a.cod_Art, a.precio1  FROM [ERPDEVLYNGT].[dbo].[Existencias] e INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo] a on a.id_art = e.Id_Art WHERE  e.Existencia_Deta_Art > 0  and Id_Bod = " & bodega

        Dim result As List(Of productos) = New List(Of productos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As productos = New productos()
                Elemento.id = TablaEncabezado.Rows(i).Item("id_art")
                Elemento.cantidad = TablaEncabezado.Rows(i).Item("Existencia_Deta_Art")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art")
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art").ToString
                Elemento.precio = TablaEncabezado.Rows(i).Item("precio1")
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerExistenciasPorCodigo(ByVal bodega As Integer, ByVal codigoproducto As String) As IList(Of productos)

        Dim sql As String = "SELECT a.id_art, e.Existencia_Deta_Art, a.Des_Art, a.cod_Art, a.precio1  FROM [ERPDEVLYNGT].[dbo].[Existencias] e INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo] a on a.id_art = e.Id_Art WHERE Id_Bod = " & bodega & " and a.cod_Art = '" & codigoproducto & "' "

        Dim result As List(Of productos) = New List(Of productos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As productos = New productos()
                Elemento.id = TablaEncabezado.Rows(i).Item("id_art")
                Elemento.cantidad = TablaEncabezado.Rows(i).Item("Existencia_Deta_Art")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art")
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art").ToString
                Elemento.precio = TablaEncabezado.Rows(i).Item("precio1")
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerBodegasDiferentesA(ByVal bodega As Integer) As IList(Of productos)

        Dim sql As String = "SELECT [Id_Bod],[Id_suc],[Id_Empsa],[Nom_Bod],[Observ_Bod],[estado],[principal],[receptora],[consignacion] FROM [ERPDEVLYNGT].[dbo].[Bodegas] where estado = 1 and   Id_Bod <> " & bodega

        Dim result As List(Of productos) = New List(Of productos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As productos = New productos()
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Bod")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_Bod")
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function



    'compras en el interior
    <WebMethod()>
    Public Function Trasladar(ByVal usuario As String, ByVal observacion As String, ByVal traslado As List(Of trasladolist)) As String

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
            Dim sql As String = "INSERT INTO [dbo].[ENC_TRASLADO] ([Fecha],[Usuario],[Observaciones],[id_empresa]) VALUES (getdate(),'" & usuario & "','" & observacion & "',(select id_empresa from USUARIO where USUARIO = '" & usuario & "'))"

            'ejecuto primer comando sql
            comando.CommandText = sql
            comando.ExecuteNonQuery()


            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()


            For Each item As trasladolist In traslado

                'INSERTA LOS DATOS 
                Dim sql2 As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[DET_TRASLADO]([IdTraslado],[id_art],[Cantidad],[IdBodOrigen],[IdBodDestino]) VALUES(" & id & "," & item.id & "," & item.cantidad & "," & item.origen & "," & item.destino & ")"

                Dim cantidad As Integer = ObtenerCantidadProducto(item.id, item.destino)
                Dim precio As Double = ObtenerCostoActual(item.id)
                Dim sql3 = ""
                If cantidad = 0 Then

                    sql3 = "INSERT INTO [dbo].[Existencias]([Id_Bod],[Id_Art],[Existencia_Deta_Art],[costoAnt]) VALUES(" & item.destino & ", " & item.id & "," & item.cantidad & ", " & precio & "); " &
                     "UPDATE [dbo].[Existencias] SET [Existencia_Deta_Art] = " & ObtenerCantidadProducto(item.id, item.origen) - item.cantidad & " WHERE [Id_Bod] = " & item.origen & " and   Id_Art = " & item.id & "; "
                Else


                    sql3 = "UPDATE [ERPDEVLYNGT].[dbo].[Existencias] SET Existencia_Deta_Art =  " & (cantidad + item.cantidad) & ",  costoAnt =  " & precio & "  WHERE [Id_Bod] = " & item.destino & " and   Id_Art = " & item.id & "; " &
                     "UPDATE [dbo].[Existencias] SET [Existencia_Deta_Art] = " & (ObtenerCantidadProducto(item.id, item.origen) - item.cantidad) & " WHERE [Id_Bod] = " & item.origen & " and   Id_Art = " & item.id & "; "
                End If


                'ejecuto segundo comando sql
                comando.CommandText = sql2
                comando.ExecuteNonQuery()


                'ejecuto tercer comando sql
                comando.CommandText = sql3
                comando.ExecuteNonQuery()
            Next

            transaccion.Commit()
            result = "SUCCESS|COMPRA GENERADA EXITOSAMENTE"

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
    Public Function ObtenerCostoActual(ByVal idart As Integer) As Double
        Dim SQL As String = "SELECT e.Id_Art, (e.Existencia_Deta_Art* a.costo_art) as costo FROM [ERPDEVLYNGT].[dbo].[Existencias] e INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo]  a ON  a.id_art = e.Id_Art where a.id_art  = " & idart

        Dim result As Double = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("costo")
            Next
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer, ByVal idbodega As Integer) As Integer
        Dim SQL As String = "Select count(Existencia_Deta_Art) as cantidad from ERPDEVLYNGT.dbo.Existencias where Id_Art = " & idart & " and id_bod = " & idbodega

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
        Public descripcion As String
        Public codigo As String
        Public cantidad As Integer
        Public precio As Double
    End Class

    Public Class trasladolist
        Public id As Integer
        Public cantidad As Integer
        Public codigo As String
        Public descripcion As String
        Public bo1 As String
        Public bo2 As String
        Public origen As Integer
        Public destino As Integer
        Public observacion As String
    End Class


End Class
