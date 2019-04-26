Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wscuentasxcobrarcl
    Inherits System.Web.Services.WebService
    'metodo utilizado para obtener el listado de productos
    <WebMethod()> _
    Public Function consultar() As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT " &
                                                "c.Nom_clt, " &
                                                "ISNULL((SELECT SUM(factura - recibos - notasCredito) FROM (SELECT " &
                                                "DATEDIFF(DAY, ef.Fecha, SYSDATETIME()) dias, " &
                                                "ISNULL((SELECT SUM((Cantidad_Articulo * Precio_Unit_Articulo) - df.Descuento) FROM DET_FACTURA df WHERE df.id_enc = ef.id_enc), 0) factura, " &
                                                "ISNULL((SELECT SUM(abonado) FROM DET_RECIBO_FACT drf INNER JOIN ENC_RECIBO er ON er.idRecibo = drf.idRecibo WHERE er.estado = 1 AND drf.id_enc = ef.id_enc), 0) recibos, " &
                                                "ISNULL((SELECT SUM(total) FROM ( " &
                                                "SELECT " &
                                                "(SELECT SUM(sumar) FROM (SELECT CASE WHEN devolucion = 0 THEN dnc.descuento ELSE (dnc.devolucion * df.Precio_Unit_Articulo) - df.Descuento END sumar FROM DET_NOTA_CREDITO dnc " &
                                                "INNER JOIN DET_FACTURA df ON df.Id_detalle = dnc.id_detalle " &
                                                "WHERE dnc.idNota = enc.idNota " &
                                                ")SUMARD) - ef2.Total_Descuento total " &
                                                "FROM ENC_NOTA_CREDITO enc " &
                                                "INNER JOIN ENC_FACTURA ef2 on enc.id_enc = ef2.id_enc " &
                                                "WHERE ef2.id_enc = ef.id_enc" &
                                                ")agrupar), 0)notasCredito, id_enc " &
                                                "FROM ENC_FACTURA ef WHERE ef.Id_Clt = c.Id_Clt and ef.efectivo IS NULL) filtrar " &
                                                "WHERE dias between 0 and 30 AND factura > (recibos + notasCredito)), 0)," &
                                                "ISNULL((SELECT SUM(factura - recibos - notasCredito) FROM (SELECT " &
                                                "DATEDIFF(DAY, ef.Fecha, SYSDATETIME()) dias, " &
                                                "ISNULL((SELECT SUM((Cantidad_Articulo * Precio_Unit_Articulo) - df.Descuento) FROM DET_FACTURA df WHERE df.id_enc = ef.id_enc), 0) factura, " &
                                                "ISNULL((SELECT SUM(abonado) FROM DET_RECIBO_FACT drf INNER JOIN ENC_RECIBO er ON er.idRecibo = drf.idRecibo WHERE er.estado = 1 AND drf.id_enc = ef.id_enc), 0) recibos, " &
                                                "ISNULL((SELECT SUM(total) FROM ( " &
                                                "SELECT  " &
                                                "(SELECT SUM(sumar) FROM (SELECT CASE WHEN devolucion = 0 THEN dnc.descuento ELSE (dnc.devolucion * df.Precio_Unit_Articulo) - df.Descuento END sumar FROM DET_NOTA_CREDITO dnc " &
                                                "INNER JOIN DET_FACTURA df ON df.Id_detalle = dnc.id_detalle " &
                                                "WHERE dnc.idNota = enc.idNota " &
                                                ")SUMARD) - ef2.Total_Descuento total " &
                                                "FROM ENC_NOTA_CREDITO enc " &
                                                "INNER JOIN ENC_FACTURA ef2 on enc.id_enc = ef2.id_enc " &
                                                "WHERE ef2.id_enc = ef.id_enc " &
                                                ")agrupar), 0)notasCredito, id_enc " &
                                                "FROM ENC_FACTURA ef WHERE ef.Id_Clt = c.Id_Clt and ef.efectivo IS NULL) filtrar " &
                                                "WHERE dias between 31 and 60 AND factura > (recibos + notasCredito)), 0), " &
                                                "ISNULL((SELECT SUM(factura - recibos - notasCredito) FROM (SELECT " &
                                                "DATEDIFF(DAY, ef.Fecha, SYSDATETIME()) dias, " &
                                                "ISNULL((SELECT SUM((Cantidad_Articulo * Precio_Unit_Articulo) - df.Descuento) FROM DET_FACTURA df WHERE df.id_enc = ef.id_enc), 0) factura, " &
                                                "ISNULL((SELECT SUM(abonado) FROM DET_RECIBO_FACT drf INNER JOIN ENC_RECIBO er ON er.idRecibo = drf.idRecibo WHERE er.estado = 1 AND drf.id_enc = ef.id_enc), 0) recibos, " &
                                                "ISNULL((SELECT SUM(total) FROM ( " &
                                                "SELECT " &
                                                "(SELECT SUM(sumar) FROM (SELECT CASE WHEN devolucion = 0 THEN dnc.descuento ELSE (dnc.devolucion * df.Precio_Unit_Articulo) - df.Descuento END sumar FROM DET_NOTA_CREDITO dnc " &
                                                "INNER JOIN DET_FACTURA df ON df.Id_detalle = dnc.id_detalle " &
                                                "WHERE dnc.idNota = enc.idNota " &
                                                ")SUMARD) - ef2.Total_Descuento total " &
                                                "FROM ENC_NOTA_CREDITO enc " &
                                                "INNER JOIN ENC_FACTURA ef2 on enc.id_enc = ef2.id_enc " &
                                                "WHERE ef2.id_enc = ef.id_enc " &
                                                ")agrupar), 0)notasCredito, id_enc " &
                                                "FROM ENC_FACTURA ef WHERE ef.Id_Clt = c.Id_Clt and ef.efectivo IS NULL) filtrar " &
                                                "WHERE dias between 61 and 90 AND factura > (recibos + notasCredito)), 0), " &
                                                "ISNULL((SELECT SUM(factura - recibos - notasCredito) FROM (SELECT " &
                                                "DATEDIFF(DAY, ef.Fecha, SYSDATETIME()) dias, " &
                                                "ISNULL((SELECT SUM((Cantidad_Articulo * Precio_Unit_Articulo) - df.Descuento) FROM DET_FACTURA df WHERE df.id_enc = ef.id_enc), 0) factura, " &
                                                "ISNULL((SELECT SUM(abonado) FROM DET_RECIBO_FACT drf INNER JOIN ENC_RECIBO er ON er.idRecibo = drf.idRecibo WHERE er.estado = 1 AND drf.id_enc = ef.id_enc), 0) recibos, " &
                                                "ISNULL((SELECT SUM(total) FROM ( " &
                                                "SELECT " &
                                                "(SELECT SUM(sumar) FROM (SELECT CASE WHEN devolucion = 0 THEN dnc.descuento ELSE (dnc.devolucion * df.Precio_Unit_Articulo) - df.Descuento END sumar FROM DET_NOTA_CREDITO dnc " &
                                                "INNER JOIN DET_FACTURA df ON df.Id_detalle = dnc.id_detalle " &
                                                "WHERE dnc.idNota = enc.idNota " &
                                                ")SUMARD) - ef2.Total_Descuento total " &
                                                "FROM ENC_NOTA_CREDITO enc " &
                                                "INNER JOIN ENC_FACTURA ef2 on enc.id_enc = ef2.id_enc " &
                                                "WHERE ef2.id_enc = ef.id_enc " &
                                                ")agrupar), 0)notasCredito, id_enc " &
                                                "FROM ENC_FACTURA ef WHERE ef.Id_Clt = c.Id_Clt and ef.efectivo IS NULL AND ef.fecha > '2015-11-01') filtrar " &
                                                "WHERE dias > 90 AND factura > (recibos + notasCredito)), 0), c.Id_Clt " &
                                                "FROM CLiente c"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For Each row As DataRow In TablaEncabezado.Rows
            Dim elemento As New Datos
            elemento.sucursal = row(0).ToString
            elemento.treinta = Convert.ToDouble(row(1).ToString).ToString("#,##0.00")
            elemento.sesenta = Convert.ToDouble(row(2).ToString).ToString("#,##0.00")
            elemento.noventa = Convert.ToDouble(row(3).ToString).ToString("#,##0.00")
            elemento.noventamas = Convert.ToDouble(row(4).ToString).ToString("#,##0.00")

            elemento.idsuc = row(5).ToString
            result.Add(elemento)
        Next
        Return result
    End Function


    'metodo utilizado para obtener el detalle de las cuentas por cobrar
    <WebMethod()> _
    Public Function getDetalle(ByVal sucursal As Integer, ByVal dias As Integer) As List(Of [Datos])
        Dim retorno As List(Of [Datos]) = New List(Of Datos)()

        Dim filtroDias As String = ""

        If dias = 1 Then
            filtroDias = "between 0 and 30"
        ElseIf dias = 2 Then
            filtroDias = "between 31 and 60"
        ElseIf dias = 3 Then
            filtroDias = "between 61 and 90"
        ElseIf dias = 4 Then
            filtroDias = "> 90"
        End If

        Dim strBusqueda As String = "SELECT firma, Nom_clt cliente, CONVERT(varchar(10), Fecha, 103) fecha, SUM(factura - recibos - notasCredito), dias, '' paciente FROM (SELECT " &
                                    "DATEDIFF(DAY, ef.Fecha, SYSDATETIME()) dias, " &
                                    "ISNULL((SELECT SUM((Cantidad_Articulo * Precio_Unit_Articulo) - df.Descuento) FROM DET_FACTURA df WHERE df.id_enc = ef.id_enc), 0) factura, " &
                                    "ISNULL((SELECT SUM(abonado) FROM DET_RECIBO_FACT drf INNER JOIN ENC_RECIBO er ON er.idRecibo = drf.idRecibo WHERE er.estado = 1 AND drf.id_enc = ef.id_enc), 0) recibos, " &
                                    "ISNULL((SELECT SUM(total) FROM ( " &
                                    "SELECT " &
                                    "(SELECT SUM(sumar) FROM (SELECT CASE WHEN devolucion = 0 THEN dnc.descuento ELSE (dnc.devolucion * df.Precio_Unit_Articulo) - df.Descuento END sumar FROM DET_NOTA_CREDITO dnc " &
                                    "INNER JOIN DET_FACTURA df ON df.Id_detalle = dnc.id_detalle " &
                                    "WHERE dnc.idNota = enc.idNota " &
                                    ")SUMARD) - ef2.Total_Descuento total " &
                                    "FROM ENC_NOTA_CREDITO enc " &
                                    "INNER JOIN ENC_FACTURA ef2 on enc.id_enc = ef2.id_enc " &
                                    "WHERE ef2.id_enc = ef.id_enc " &
                                    ")agrupar), 0)notasCredito, id_enc, ef.firma, ef.Fecha, c.Nom_clt " &
                                    "FROM ENC_FACTURA ef " &
                                    "INNER JOIN CLiente c ON ef.Id_Clt = c.Id_Clt " &
                                    "WHERE ef.Id_Clt = " & sucursal & " and ef.efectivo IS NULL) filtrar " &
                                    "WHERE dias " & filtroDias & " AND factura > (recibos + notasCredito) " &
                                    "GROUP BY firma, Nom_clt, Fecha, dias"
        Dim tblBusqueda As DataTable = manipular.Llena_Drop(strBusqueda)
        Dim tot As Double = 0
        For Each row As DataRow In tblBusqueda.Rows

            Dim ele As New Datos
            tot += Convert.ToDouble(row(3).ToString)
            ele.factura = row(0).ToString
            ele.cliente = row(1).ToString
            ele.fecha = row(2).ToString
            ele.saldo = Convert.ToDouble(row(3).ToString).ToString("#,##0.00")
            ele.total = tot.ToString("#,##0.00")
            ele.dias = row(4).ToString
            ele.paciente = row(5).ToString

            retorno.Add(ele)

        Next

        Return retorno
    End Function

    Public Class Datos
        Public factura As String
        Public cliente As String
        Public paciente As String
        Public fecha As String
        Public saldo As String
        Public dias As String
        Public sucursal As String
        Public treinta As String
        Public total As String
        Public sesenta As String
        Public noventa As String
        Public noventamas As String
        Public idsuc As String
    End Class

End Class