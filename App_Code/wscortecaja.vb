Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wscortecaja
    Inherits System.Web.Services.WebService

    'metodo utilizado para obtener la fecha cierre
    <WebMethod()> _
    Public Function getFechaCierre() As String
        Dim result As String = ""
        Dim fCorresponde As String = manipular.idempresabusca("SELECT CONVERT(varchar(10), DATEADD(DAY, 1, MAX(fCorresponde)), 103) FROM CIERRESUC WHERE id_suc = 1  AND estado = 1")
        result = fCorresponde
        Return result
    End Function

    'metodo utilizado para generar el cierre de caja
    <WebMethod(True)> _
    Public Function generarCierre(ByVal fCierre As String) As String

        Dim retorno As String = Nothing

        Dim fechass As String() = fCierre.Split("/")
        fCierre = fechass(2) & "-" & fechass(1) & "-" & fechass(0)

        Dim strInsert As String = "INSERT INTO CIERRESUC " &
                                  "(fCorresponde, usuario, id_suc) " &
                                  "VALUES " &
                                  "('" & fCierre & "', 'admin1', 1)"
        Dim insertado As Boolean = manipular.EjecutaTransaccion1(strInsert)

        If insertado Then

            Dim idInsertado As Integer = manipular.idempresabusca("SELECT MAX(idCierre) FROM CIERRESUC WHERE fCorresponde = '" & fCierre & "' AND estado = 1")

            Dim strDetalle As String = "INSERT INTO DETCIERRESUC " &
                                       "(idCierre, efectivo, visa, credomatic, cheques) " &
                                       "SELECT " &
                                       "" & idInsertado & ", ISNULL(SUM(ef.efectivo), 0) efectivo, ISNULL((SELECT SUM(ef.tarjeta) tarjeta FROM ENC_FACTURA ef WHERE CAST(Fecha as date) between '" & fCierre & "' AND '" & fCierre & "' AND (tipoTarjeta = 1 OR tipoTarjeta IS NULL)), 0) visa, ISNULL((SELECT SUM(ef.tarjeta) tarjeta FROM ENC_FACTURA ef WHERE CAST(Fecha as date) between '" & fCierre & "' AND '" & fCierre & "' AND (tipoTarjeta = 2)), 0) credomatic, ISNULL(SUM(ef.cheques), 0) cheque " &
                                       "FROM ENC_FACTURA ef " &
                                       "WHERE CAST(Fecha as date) between '" & fCierre & "' AND '" & fCierre & "' AND ef.id_suc =1"
            manipular.EjecutaTransaccion1(strDetalle)

            retorno = manipular.idempresabusca("SELECT CONVERT(varchar(10), DATEADD(DAY, 1, fCorresponde), 103) FROM CIERRESUC WHERE idCierre = " & idInsertado)

        Else
            retorno = "E"
        End If

        Return retorno

    End Function


    'metodo utilizado para consultar el resumen de corte de caja
    <WebMethod()> _
    Public Function resumen(ByVal fechaIni As String, ByVal fechaFin As String, ByVal suc As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim tot As Double = 0
        Dim res As Double = 0

        Dim fCorresponde As String = manipular.idempresabusca("SELECT CONVERT(varchar(10), DATEADD(DAY, 1, MAX(fCorresponde)), 103) FROM CIERRESUC WHERE id_suc = 1  AND estado = 1")


        Dim StrEncabezado As String = "SELECT " &
                                      "ISNULL(SUM(ef.efectivo), 0) efectivo, " &
                                      "(ISNULL((SELECT SUM(ef.tarjeta) tarjeta FROM ENC_FACTURA ef WHERE CAST(Fecha as date) between '" + fechaIni + "' AND '" + fechaFin + "' AND ef.id_suc = " & suc & " AND (tipoTarjeta = 1 OR tipoTarjeta IS NULL)), 0) + " &
                                      "ISNULL((SELECT SUM(ef.tarjeta2) tarjeta FROM ENC_FACTURA ef WHERE CAST(Fecha as date) between '" + fechaIni + "' AND '" + fechaFin + "' AND (tipoTarjeta2 = " & suc & " OR tipoTarjeta2 IS NULL)), 0)) visa, " &
                                      "(ISNULL((SELECT SUM(ef.tarjeta) tarjeta FROM ENC_FACTURA ef WHERE CAST(Fecha as date) between '" + fechaIni + "' AND '" + fechaFin + "' AND ef.id_suc = " & suc & " AND (tipoTarjeta = 2)), 0) + " &
                                      "ISNULL((SELECT SUM(ef.tarjeta2) tarjeta FROM ENC_FACTURA ef WHERE CAST(Fecha as date) between '" + fechaIni + "' AND '" + fechaFin + "' AND ef.id_suc = " & suc & " AND (tipoTarjeta2 = 2)), 0)) credomatic, " &
                                      " ISNULL(SUM(ef.cheques), 0) cheque, " &
                                      "  ISNULL(CAST((SELECT SUM(total) FROM ( SELECT CAST((SELECT " &
                                      " CASE WHEN SUM(dnc.descuento) = 0 THEN SUM((devolucion * df.Precio_Unit_Articulo) - df.Descuento) - ef.Total_Descuento ELSE SUM(dnc.descuento) END " &
                                      " FROM DET_NOTA_CREDITO dnc " &
                                      " INNER JOIN DET_FACTURA df ON dnc.id_detalle = df.Id_detalle " &
                                      "  WHERE dnc.idNota = enc.idNota) as numeric(18, 2)) total " &
                                      "  FROM ENC_FACTURA ef " &
                                      "  INNER JOIN CLiente c ON ef.Id_Clt = c.Id_Clt " &
                                      "  FULL JOIN USUARIO u ON ef.USUARIO = u.USUARIO " &
                                      "  INNER JOIN ENC_NOTA_CREDITO enc ON enc.id_enc = ef.id_enc " &
                                      "  WHERE CAST(enc.Fecha as date) between '" & fechaIni & "' AND '" & fechaFin & "' AND ef.id_suc = " & suc &
                                      "  )agrupar) as numeric(18, 2)), 0) nc, " &
                                      "  ISNULL(SUM(ef.empleado), 0) empleado, " &
                                      "  ISNULL(SUM(ef.empresa), 0) empresa " &
                                      "  FROM ENC_FACTURA ef " &
                                      "  WHERE CAST(Fecha as date) between '" & fechaIni & "' AND '" & fechaFin & "' AND ef.id_suc =" & suc
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.efectivo = Convert.ToDouble(TablaEncabezado.Rows(i).Item("efectivo")).ToString
                Elemento.visa = Convert.ToDouble(TablaEncabezado.Rows(i).Item("visa")).ToString
                Elemento.credomatic = Convert.ToDouble(TablaEncabezado.Rows(i).Item("credomatic")).ToString
                Elemento.cheque = Convert.ToDouble(TablaEncabezado.Rows(i).Item("cheque")).ToString
                Elemento.notaCredito = Convert.ToDouble(TablaEncabezado.Rows(i).Item("nc")).ToString
                Elemento.descuentoE = Convert.ToDouble(TablaEncabezado.Rows(i).Item("empleado")).ToString
                Elemento.empresasAs = Convert.ToDouble(TablaEncabezado.Rows(i).Item("empresa")).ToString
                tot += (Convert.ToDouble(TablaEncabezado.Rows(i).Item("efectivo")).ToString + Convert.ToDouble(TablaEncabezado.Rows(i).Item("visa")).ToString + Convert.ToDouble(TablaEncabezado.Rows(i).Item("credomatic")).ToString + Convert.ToDouble(TablaEncabezado.Rows(i).Item("cheque")).ToString + Convert.ToDouble(TablaEncabezado.Rows(i).Item("nc")).ToString + Convert.ToDouble(TablaEncabezado.Rows(i).Item("empleado")).ToString + Convert.ToDouble(TablaEncabezado.Rows(i).Item("empresa")).ToString)
                res += Convert.ToDouble(TablaEncabezado.Rows(i).Item("nc")).ToString
                Elemento.subtotal = tot
                Elemento.fechaCierre = fCorresponde
                Elemento.total = (tot - res)

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para consultar el listado de recibos
    <WebMethod()> _
    Public Function Consultar(ByVal fechaIni As String, ByVal fechaFin As String, ByVal suc As Integer) As List(Of [Datos])
        Dim outIva As Double = 0
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT " &
                                      "Serie_Fact, firma, c.nit_clt, c.Nom_clt + ISNULL((SELECT '/' + RTRIM(LTRIM(Nombres)) + ' ' + RTRIM(LTRIM(Apellidos)) FROM ORDENTRABAJO WHERE idOrdenTrabajo = ef.idOrdenTrabajo), '') nombreC, " &
                                      "CAST(ef.idOrdenTrabajo as varchar) + CAST((SELECT ISNULL('/' + CASE WHEN estado = 5 THEN 'Pendiente de Recepcion' WHEN estado = 6 THEN 'En Tienda' WHEN estado = 7 THEN 'Entregado' ELSE 'En Proceso' END, '') FROM ORDENTRABAJO WHERE idOrdenTrabajo = ef.idOrdenTrabajo) as varchar) orden, " &
                                      "ISNULL(ef.efectivo, 0) efectivo, CASE WHEN tipoTarjeta = 1 THEN ISNULL(ef.tarjeta, 0) ELSE 0 END + CASE WHEN tipoTarjeta2 = 1 THEN ISNULL(ef.tarjeta2, 0) ELSE 0 END visa, ISNULL(ef.cheques, 0) cheques, " &
                                      "ISNULL(CAST((SELECT ISNULL(SUM((Cantidad_Articulo * Precio_Unit_Articulo) - Descuento), 0) FROM DET_FACTURA WHERE id_enc = ef.id_enc) as numeric(18, 2)) - Total_Descuento, 0) total, " &
                                      "CASE WHEN tipoTarjeta = 2 THEN ISNULL(ef.tarjeta, 0) ELSE 0 END + CASE WHEN tipoTarjeta2 = 2 THEN ISNULL(ef.tarjeta2, 0) ELSE 0 END credomatic, ef.id_enc, u.Nombres + ' ' + u.Apellidos nombre, " &
                                      "CASE WHEN ef.idOrdenTrabajo IS NULL AND ef.complemento IS NULL THEN 'Venta Directa'  WHEN ef.idOrdenTrabajo IS NOT NULL AND ef.complemento IS NOT NULL THEN 'Complemento' ELSE CASE WHEN (SELECT SUM((precio * cantidad) - descuento) FROM DET_PEDIDO_ORDEN WHERE idPedidoOrden = (SELECT MIN(idPedidoOrden) FROM ENC_PEDIDO_ORDEN WHERE idOrdenTrabajo = ef.idOrdenTrabajo)) = CAST((SELECT ISNULL(SUM((Cantidad_Articulo * Precio_Unit_Articulo) - Descuento), 0) FROM DET_FACTURA WHERE id_enc = ef.id_enc) as numeric(18, 2)) - Total_Descuento THEN 'Cancelación' ELSE 'Anticipo' END END tipo, complemento, ISNULL(ef.empleado, 0) empleado, ISNULL(ef.empresa, 0) empresa " &
                                      "FROM ENC_FACTURA ef " &
                                      "INNER JOIN CLiente c ON ef.Id_Clt = c.Id_Clt " &
                                      "FULL JOIN USUARIO u ON ef.USUARIO = u.USUARIO " &
                                      "WHERE CAST(Fecha as date) between '" & fechaIni & "' AND '" & fechaFin & "' AND ef.id_suc = " & suc &
                                      " UNION ALL " &
                                      "Select " &
                                      "enc.idSerieNota, enc.firma, c.nit_clt, c.Nom_clt + ISNULL((SELECT '/' + RTRIM(LTRIM(Nombres)) + ' ' + RTRIM(LTRIM(Apellidos)) FROM ORDENTRABAJO WHERE idOrdenTrabajo = ef.idOrdenTrabajo), ''), '', 0, 0, 0, " &
                                      "CAST((SELECT " &
                                      "CASE WHEN SUM(dnc.descuento) = 0 THEN SUM((devolucion * df.Precio_Unit_Articulo) - df.Descuento) - ef.Total_Descuento ELSE SUM(dnc.descuento) END " &
                                      "FROM DET_NOTA_CREDITO dnc " &
                                      "INNER JOIN DET_FACTURA df ON dnc.id_detalle = df.Id_detalle " &
                                      "WHERE dnc.idNota = enc.idNota) as numeric(18, 2)) total, 0 credomatic, ef.id_enc, u.Nombres + ' ' + u.Apellidos nombre, " &
                                      "CASE WHEN enc.esAbono IS NOT NULL THEN 'N/A' ELSE 'N/C' END, complemento, " &
                                      "0 empleado, 0 empresa " &
                                      "FROM ENC_FACTURA ef " &
                                      "INNER JOIN CLiente c ON ef.Id_Clt = c.Id_Clt " &
                                      "FULL JOIN USUARIO u ON ef.USUARIO = u.USUARIO " &
                                      "INNER JOIN ENC_NOTA_CREDITO enc ON enc.id_enc = ef.id_enc " &
                                      "WHERE CAST(enc.Fecha as date) between '" & fechaIni & "' AND '" & fechaFin & "' AND ef.id_suc = " & suc &
                                      " ORDER BY firma "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.serie = TablaEncabezado.Rows(i).Item("Serie_Fact")
                Elemento.firma = TablaEncabezado.Rows(i).Item("firma").ToString
                Elemento.nit = TablaEncabezado.Rows(i).Item("nit_clt").ToString
                Elemento.cliente = TablaEncabezado.Rows(i).Item("nombreC").ToString
                Elemento.orden = TablaEncabezado.Rows(i).Item("orden").ToString
                Elemento.efectivo = Convert.ToDouble(TablaEncabezado.Rows(i).Item("efectivo")).ToString
                Elemento.visa = Convert.ToDouble(TablaEncabezado.Rows(i).Item("visa")).ToString
                Elemento.cheque = Convert.ToDouble(TablaEncabezado.Rows(i).Item("cheques")).ToString
                Elemento.total = Convert.ToDouble(TablaEncabezado.Rows(i).Item("total")).ToString
                Elemento.credomatic = Convert.ToDouble(TablaEncabezado.Rows(i).Item("credomatic")).ToString
                Elemento.serie = TablaEncabezado.Rows(i).Item("id_enc")
                Elemento.usuario = TablaEncabezado.Rows(i).Item("nombre").ToString
                Elemento.tipo = TablaEncabezado.Rows(i).Item("tipo").ToString
                Elemento.empleado = TablaEncabezado.Rows(i).Item("empleado").ToString
                Elemento.empresa = TablaEncabezado.Rows(i).Item("empresa").ToString
                If TablaEncabezado.Rows(i).Item("tipo").ToString.Trim = "N/C" Then
                    outIva -= Convert.ToDouble(Convert.ToDouble(TablaEncabezado.Rows(i).Item("total")).ToString / 1.12)
                Else
                    outIva += Convert.ToDouble(Convert.ToDouble(TablaEncabezado.Rows(i).Item("total")).ToString / 1.12)
                End If
                Elemento.sinIva = outIva
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next


        Return result
    End Function

    Public Class Datos
        Public fechaCierre As String
        Public serie As Integer
        Public firma As String
        Public nit As String
        Public cliente As String
        Public orden As String
        Public efectivo As Double
        Public visa As String
        Public credomatic As Double
        Public cheque As Double
        Public empleado As String
        Public empresa As String
        Public empresasAs As Double
        Public descuentoE As Double
        Public notaCredito As Double
        Public subtotal As Double
        Public total As Double
        Public sinIva As Double
        Public usuario As String
        Public tipo As String
    End Class

End Class