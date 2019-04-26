Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsrepventa
    Inherits System.Web.Services.WebService


    'metodo utilizado parA obtener los departamentos
    <WebMethod()> _
    Public Function getClientes() As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select * from CLiente"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Clt")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_clt").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next


        Return result
    End Function

    <WebMethod()> _
    Public Function Consultar(ByVal fechaIni As String, ByVal fechaFin As String, ByVal suc As Integer, ByVal region As Integer, ByVal cliente As String) As List(Of [Datos])
        Dim filtro As String = ""

        If suc > 0 Then
            filtro = " and id_suc = " & suc
        End If


        If cliente <> "" Then
            filtro &= " and cliente = '" & cliente & "' "
        End If


        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT factura, fecha, total TOTR, notas, CASE WHEN total - notas = 0 THEN 'Anulada' WHEN notas > 0 and total - notas > 0 THEN 'NC' ELSE 'Vigente' END estado, cliente, fechaf, id_dep, id_muni, id_suc, id_region, total - notas - rec SALDO, id_enc FROM ( " &
                                    "SELECT  " &
                                    "RTRIM(Serie_Fact)  factura, CONVERT(varchar(10), CAST(fecha as date), 103) fecha, SUM(df.Sub_Total) total, " &
                                    "ISNULL((SELECT SUM(CASE WHEN devolucion > 0 THEN devolucion * Precio_Unit_Articulo ELSE dnc.descuento END) valor FROM ENC_NOTA_CREDITO enc " &
                                    "INNER JOIN DET_NOTA_CREDITO dnc ON enc.idNota = dnc.idNota " &
                                    "INNER JOIN DET_FACTURA df2 on dnc.id_detalle = df2.Id_detalle " &
                                    "WHERE enc.id_enc = ef.id_enc " &
                                    "), '0') notas , c.Nom_clt cliente, fecha fechaf, c.id_dep, c.id_muni, ef.id_suc, r.id_region, ISNULL((SELECT SUM(abonado) FROM DET_RECIBO_FACT WHERE id_enc = ef.id_enc), 0) rec, ef.id_enc " &
                                    "FROM ENC_FACTURA ef " &
                                    "INNER JOIN DET_FACTURA df on ef.id_enc = df.id_enc " &
                                    "INNER JOIN CLiente c on ef.Id_Clt = c.Id_Clt " &
                                    "INNER JOIN SUCURSALES s on ef.id_suc = s.id_suc " &
                                    "INNER JOIN REGIONES r on s.id_region = r.id_region " &
                                    "GROUP BY Serie_Fact, firma, Fecha, ef.id_enc, c.Nom_clt, c.nit_clt, c.id_dep, c.id_muni, ef.id_suc, r.id_region " &
                                    ")ORDEN WHERE CAST(fechaf as date) between '" & fechaIni & "' and '" & fechaFin & "'" & filtro
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_enc")
                Elemento.factura = TablaEncabezado.Rows(i).Item("factura").ToString
                Elemento.fecha = TablaEncabezado.Rows(i).Item("fecha").ToString
                Elemento.cliente = TablaEncabezado.Rows(i).Item("cliente").ToString
                Elemento.valor = Convert.ToDouble(TablaEncabezado.Rows(i).Item("TOTR")).ToString
                Elemento.estado = TablaEncabezado.Rows(i).Item("estado").ToString
                Elemento.saldo = Convert.ToDouble(TablaEncabezado.Rows(i).Item("SALDO")).ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next
        Return result
    End Function


    'consultar detalle 
    <WebMethod()> _
    Public Function consultarDet(ByVal id As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select b.Nom_Bod, a.cod_Art, a.Des_Art, df.Cantidad_Articulo, df.Precio_Unit_Articulo, (df.Cantidad_Articulo * df.Precio_Unit_Articulo) total " &
                                      "from DET_FACTURA df " &
                                      "JOIN Bodegas b " &
                                      "on b.Id_Bod = df.Id_Bod " &
                                      "JOIN Articulo a " &
                                      "on a.id_art = df.Id_Art " &
                                      "where df.id_enc = " & id
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.bodega = TablaEncabezado.Rows(i).Item("Nom_Bod").ToString
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art").ToString
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art").ToString
                Elemento.cantidad = TablaEncabezado.Rows(i).Item("Cantidad_Articulo")
                Elemento.precio = TablaEncabezado.Rows(i).Item("Precio_Unit_Articulo").ToString
                Elemento.total = TablaEncabezado.Rows(i).Item("total")

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public descripcion As String
        Public factura As String
        Public fecha As String
        Public cliente As String
        Public valor As Double
        Public estado As String
        Public saldo As Double
        Public bodega As String
        Public codigo As String
        Public articulo As String
        Public cantidad As Integer
        Public precio As Double
        Public total As Double
    End Class

End Class