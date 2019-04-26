Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsrepexist
    Inherits System.Web.Services.WebService

    'metodo utilizado para llenar las regiones
    <WebMethod()> _
    Public Function regiones(ByVal empresa As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select id_region, descripcion from REGIONES where id_empresa = " & empresa & " and estado = 1"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_region")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("descripcion").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para llenar las sucursales
    <WebMethod()> _
    Public Function sucursales(ByVal empresa As Integer, ByVal region As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select id_suc, descripcion from SUCURSALES where id_empresa = " & empresa & " and id_region = " & region
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("descripcion").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para llenar las bodegas
    <WebMethod()> _
    Public Function bodegas(ByVal sucursal As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT id_bod, nom_bod FROM bodegas WHERE id_suc = " & sucursal & " order by nom_bod"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_bod")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("nom_bod").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    'metodo utilizado para obtener el listado de existencias 
    <WebMethod()> _
    Public Function consultar(ByVal region As Integer, ByVal sucursal As Integer, ByVal bodega As Integer) As List(Of [Datos])
        Dim filtro = ""

        If region > 0 Then
            filtro = filtro & " and s.id_region = " & region & ""
            If sucursal > 0 Then
                filtro = filtro & " and b.Id_suc = " & sucursal & ""
                If bodega > 0 Then
                    filtro = filtro & " and e.id_bod = " & bodega & ""
                End If
            End If
        End If


        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select e.Id_Bod,e.Id_Art,e.Existencia_Deta_Art as existencia,e.Reservadas,b.Nom_Bod as bodega,a.cod_Art, a.cod_pro1,a.costo_art,A.Des_Art, b.Id_suc,s.id_region, ISNULL(c.descripcionColor, '') colorn, a.precio1, M.NOM_MARCA, " &
                                    "(select sum(cantidad_articulo) from det_or_compra d inner join ENC_OR_COMPRA e on e.id_enc = d.id_enc where e.estatus = 2 and e.estado = 0 and d.id_art = a.id_art) transito " &
                                    "from Existencias e inner join Bodegas b on b.Id_Bod = e.Id_Bod inner join Articulo a on a.id_art = e.Id_Art inner join SUCURSALES s on s.id_suc = b.Id_suc " &
                                    "FULL JOIN COLOR c ON a.idColor = c.idColor " &
                                    "FULL JOIN MARCAS M ON M.ID_MARCA = A.ID_MARCA " &
                                    "where e.Existencia_Deta_Art > 0" & filtro & " AND a.id_tipo IN (3, 1, 6, 8, 9)"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art").ToString
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art").ToString
                Elemento.marcas = TablaEncabezado.Rows(i).Item("NOM_MARCA").ToString
                Elemento.color = TablaEncabezado.Rows(i).Item("colorn").ToString
                Elemento.bodega = TablaEncabezado.Rows(i).Item("bodega").ToString
                Elemento.existencia = TablaEncabezado.Rows(i).Item("existencia")
                Elemento.costoUnit = Convert.ToDouble(TablaEncabezado.Rows(i).Item("costo_art").ToString).ToString("#,###,###,##0.00")
                Elemento.precio = Convert.ToDouble(TablaEncabezado.Rows(i).Item("precio1")).ToString("#,###,###,##0.00")
                Elemento.costoTotal = Convert.ToDouble(TablaEncabezado.Rows(i).Item("existencia") * Convert.ToDouble(TablaEncabezado.Rows(i).Item("costo_art"))).ToString("#,###,###,##0.00")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public codigo As String
        Public descripcion As String
        Public marcas As String
        Public color As String
        Public bodega As String
        Public existencia As Integer
        Public costoUnit As Double
        Public precio As Double
        Public costoTotal As Double
    End Class
End Class