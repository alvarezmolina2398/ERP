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
Public Class wscobrar_Recibos
    Inherits System.Web.Services.WebService

    'METODO PARA OBTENER LAS FACTURAS
    <WebMethod()>
    Public Function ObtenerFacturas(ByVal idcliente As Integer) As List(Of datos)

        Dim sql As String = "SELECT id_enc, Serie_Fact, firma, valor, recibos, valor - recibos saldos FROM ( " &
            " SELECT ef.id_enc, Serie_Fact, firma, SUM(df.Sub_Total) valor, ISNULL((SELECT SUM(abonado) FROM DET_RECIBO_FACT WHERE id_enc = ef.id_enc), 0) recibos " &
            " FROM ENC_FACTURA ef  " &
            " INNER JOIN DET_FACTURA df on ef.id_enc = df.id_enc  " &
            " INNER JOIN CLiente c on ef.Id_Clt = c.Id_Clt " &
            " WHERE c.Id_Clt =  " & idcliente &
            " GROUP BY ef.id_enc, Serie_Fact, firma " &
            " ) fac WHERE valor > recibos "



        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            Dim Elemento As New datos
            Elemento.id_fac = TablaEncabezado.Rows(i).Item("id_enc")
            Elemento.serie = TablaEncabezado.Rows(i).Item("Serie_Fact")
            Elemento.firma = TablaEncabezado.Rows(i).Item("firma")
            Elemento.valor = TablaEncabezado.Rows(i).Item("valor")
            Elemento.saldo = TablaEncabezado.Rows(i).Item("saldos")
            result.Add(Elemento)
        Next


        Return result
    End Function

    'METODO PARA AGREGAR EL CARGO
    <WebMethod()>
    Public Function Pagar(ByVal idcliente As Integer, ByVal usuario As String, ByVal listpagos As List(Of pagos), ByVal listfac As List(Of datos), ByVal total_abonar As Double) As String
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
            Dim empresa As String = "SELECT id_empresa  FROM  [USUARIO] where USUARIO = '" & usuario & "'"
            Dim sucursal As String = "SELECT id_sucursal  FROM  [USUARIO] where USUARIO = '" & usuario & "'"

            'INSERTAMOS RECIVO
            Dim strRecivo As String = "INSERT INTO  [ENC_RECIBO] ([fecha],[Id_Clt],[Usuario],[empresa],[sucursal],[estado]) " &
                " VALUES(GETDATE()," & idcliente & ",'" & usuario & "',(" & empresa & "),(" & sucursal & "),1);"

            comando.CommandText = strRecivo
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE RECIVO
            comando.CommandText = "SELECT @@IDENTITY"
            Dim idRecivo As Integer = comando.ExecuteScalar()

            Dim posicion As Integer = 0

            'INSERCION DEL DETALLE DEL RECIVO (METODOS DE PAGO)

            For Each item As pagos In listpagos
                Dim StrPago As String = "INSERT INTO [dbo].[DET_RECIBO]([idRecibo],[tipoPago],[documento],[valor]) " &
                    "VALUES(" & idRecivo & ",'" & item.tipo & "','" & item.informacion & "'," & item.valor & ")"
                comando.CommandText = StrPago
                comando.ExecuteNonQuery()
            Next

            Dim total_actual As Double = total_abonar

            'INGRESO DEL DETALLE DE RECIBOS
            For Each item As datos In listfac
                Dim strFac_RECIVO As String = ""
                'SI EL SALDO SERA MAYOR
                If item.saldo <= total_actual Then
                    strFac_RECIVO = "INSERT INTO  [DET_RECIBO_FACT]([idRecibo],[id_enc],[abonado]) " &
                    "VALUES(" & idRecivo & "," & item.id_fac & "," & item.saldo & ");"

                    total_actual = total_actual - item.saldo
                Else
                    'SI EL SALDO ES MENOR
                    strFac_RECIVO = "INSERT INTO  [DET_RECIBO_FACT]([idRecibo],[id_enc],[abonado]) " &
                    "VALUES(" & idRecivo & "," & item.id_fac & "," & total_actual & ");"

                    total_actual = 0
                End If



                comando.CommandText = strFac_RECIVO
                comando.ExecuteNonQuery()
            Next


            'SI EXISTE SALDO A FAVOR
            If total_actual > 0 Then
                Dim strFac_RECIVO = "INSERT INTO  [DET_RECIBO_FACT]([idRecibo],[abonado]) " &
                    "VALUES(" & idRecivo & "," & total_actual & ");"

                comando.CommandText = strFac_RECIVO
                comando.ExecuteNonQuery()

                total_actual = 0
            End If


            transaccion.Commit()
            result = "SUCCESS| DATOS FACTURADOS EXITOSAMENTE"


        Catch ex As Exception
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try


        Return result
    End Function



    Public Class datos
        Public id_fac As Integer
        Public serie As String
        Public firma As String
        Public valor As Double
        Public saldo As Double
    End Class

    Public Class pagos
        Public tipo As Integer
        Public valor As Double
        Public informacion As String
        Public tipoPagoText As String
        Public cambio As Double
    End Class

End Class