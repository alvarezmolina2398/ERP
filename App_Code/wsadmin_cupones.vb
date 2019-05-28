Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class wsadmin_cupones
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function InsertarCupones(ByVal serie As String, ByVal observacion As String, ByVal cantidad As Integer, ByVal valor As Double, ByVal idcliente As Integer, ByVal usuario As String) As String


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

            'INSERCION DE LA FACTURA
            Dim str1 As String = "INSERT INTO [dbo].[ENCA_CUPON]([serie] ,[observacion],[cantidad],[correlativo],[estado],[idcliente],[usuario]) VALUES " &
            "('" & serie & "', '" & observacion & "'," & cantidad & "," & cantidad & ",1," & idcliente & ",'" & usuario & "')"

            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()

            For index As Integer = 1 To cantidad
                Dim ser As String = serie & "-" & index
                Dim str2 As String = "INSERT INTO [dbo].[DETA_CUPON]([id_cupon],[valor],[saldo],[serie],[estado]) VALUES" &
                    "(" & id & "," & valor & "," & valor & ",'" & ser & "',1)"


                comando.CommandText = str2
                comando.ExecuteNonQuery()
            Next

            transaccion.Commit()
            result = "SUCCESS|CUPONES CREADOS EXITOSAMENTE  "
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
    Public Function ObtenerCorrelatiivo(ByVal id As Integer) As Integer
        Dim SQL As String = "SELECT correlativo  from  [ENCA_CUPON] WHERE id_cupon = " & id

        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("correlativo")
            Next
        Next

        Return result

    End Function



    <WebMethod()>
    Public Function InsertarCuponesASerie(ByVal serie As String, ByVal cantidad As Integer, ByVal valor As Double, ByVal id As Integer) As String


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
            Dim total As String = "(SELECT correlativo + " & cantidad & " from  [ENCA_CUPON] WHERE id_cupon = " & id & ")"
            Dim sql As String = "UPDATE  [ENCA_CUPON] SET correlativo = " & total & " where id_cupon = " & id

            Dim correlativo As Integer = ObtenerCorrelatiivo(id)

            comando.CommandText = sql
            comando.ExecuteNonQuery()



            For index As Integer = 1 To cantidad
                Dim ser As String = serie & "-" & (correlativo + index)
                Dim str2 As String = "INSERT INTO  [DETA_CUPON]([id_cupon], [valor], [saldo], [serie], [estado]) VALUES" &
                    "(" & id & "," & valor & "," & valor & ",'" & ser & "',1);"


                comando.CommandText = str2
                comando.ExecuteNonQuery()
            Next

            transaccion.Commit()
            result = "SUCCESS|CUPONES CREADOS EXITOSAMENTE  "
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
    Public Function ObtenerDatos() As List(Of datos)
        Dim sql = "SELECT  C.id_cupon,C.serie,C.observacion,C.cantidad,C.correlativo,C.estado,C.idcliente,cl.Nom_clt,C.usuario, cl.nit_clt FROM  [ENCA_CUPON] C INNER JOIN  [CLiente] Cl ON Cl.Id_Clt = c.idcliente  WHERE c.estado = 1"
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            Dim Elemento As New datos
            Elemento.id = TablaEncabezado.Rows(i).Item("id_cupon")
            Elemento.observacion = TablaEncabezado.Rows(i).Item("observacion")
            Elemento.serie = TablaEncabezado.Rows(i).Item("serie")
            Elemento.cliente = TablaEncabezado.Rows(i).Item("Nom_clt")
            Elemento.nit = TablaEncabezado.Rows(i).Item("nit_clt")
            Elemento.idcliente = TablaEncabezado.Rows(i).Item("idcliente")
            Elemento.correlativo = TablaEncabezado.Rows(i).Item("correlativo")
            result.Add(Elemento)
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function ObtenerDetalleCupones(ByVal id As Integer) As List(Of detalle)
        Dim sql = "SELECT [id_cupon_detalle],[id_cupon],[valor],[saldo],[serie],[estado] FROM  [DETA_CUPON] WHERE estado  = 1 and id_cupon = " & id & " order by id_cupon_detalle  asc "
        Dim result As List(Of detalle) = New List(Of detalle)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            Dim Elemento As New detalle
            Elemento.id = TablaEncabezado.Rows(i).Item("id_cupon_detalle")
            Elemento.serie = TablaEncabezado.Rows(i).Item("serie")
            Elemento.valor = TablaEncabezado.Rows(i).Item("valor")
            Elemento.saldo = TablaEncabezado.Rows(i).Item("saldo")
            result.Add(Elemento)
        Next

        Return result
    End Function


    Public Class datos
        Public id As Integer
        Public observacion As String
        Public serie As String
        Public cliente As String
        Public nit As String
        Public idcliente As Integer
        Public correlativo As Integer
    End Class

    Public Class detalle
        Public id As Integer
        Public serie As String
        Public valor As String
        Public saldo As String

    End Class

End Class