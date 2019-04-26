Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsrepventaproducto
     Inherits System.Web.Services.WebService

    'metodo utilizado para llenar el tipo de producto
    <WebMethod()> _
    Public Function getTipo() As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select idtipolente,tipolente from tipoarticulo where tipolente <> '' "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idtipolente")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("tipolente").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    'metodo utilizado para llenar las sucursales
    <WebMethod()> _
    Public Function sucursales(ByVal empresa As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select id_suc, descripcion from SUCURSALES where id_empresa = " & empresa
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

    'metodo utilizado para obtener el listado de productos
    <WebMethod()> _
    Public Function getProductos() As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select * from Articulo"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_art")
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function





    'metodo utilizado para obtener el listado de compras 
    <WebMethod()> _
    Public Function consultar(ByVal fechaIn As String, ByVal FechaFin As String, ByVal sucursal As Integer, ByVal tipo As Integer, ByVal codigo As String) As List(Of [Datos])
        Dim filtro = ""
        Dim filtro2 = ""

        If sucursal = "0" Then
            filtro &= ""
        Else
            filtro &= " and ef.id_suc = " & sucursal
        End If

        If tipo = "0" Then
            filtro &= ""
        Else
            filtro &= " and a.id_tipo = " & tipo & ""
        End If

        If codigo <> "" Then
            filtro &= " and a.cod_Art= '" & codigo & "'"
        End If

        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT a.cod_Art, a.Des_Art, df.Cantidad_Articulo, B.Nom_Bod, ef.firma factura, a.id_tipo, tp.tipolente, CONVERT(varchar(10), CAST(ef.Fecha as date), 103)  fechac " &
                                    ",((df.Cantidad_Articulo*df.precio_unit_articulo)-df.descuento) valor,co.descripcionColor,ef.usuario,mar.nom_marca FROM ENC_FACTURA ef " &
                                    "INNER JOIN DET_FACTURA df ON ef.id_enc = df.id_enc " &
                                    "INNER JOIN Articulo a ON df.Id_Art = a.id_art " &
                                    "full join COLOR co on co.idColor = a.idcolor " &
                                    "full join MARCAS mar on mar.id_marca = a.id_marca " &
                                    "FULL JOIN Bodegas b ON b.Id_Bod = df.Id_Bod " &
                                    "full join tipoarticulo tp on tp.idtipolente = a.id_tipo " &
                                    "INNER JOIN CLiente c on ef.Id_Clt = c.Id_Clt " &
                                    "INNER JOIN SUCURSALES s on ef.id_suc = s.id_suc " &
                                    "INNER JOIN REGIONES r on s.id_region = r.id_region " &
                                    "WHERE CAST(ef.Fecha as date) between '" & fechaIn & "' and '" & FechaFin & "' " & filtro &
                                    " ORDER BY LTRIM(Des_Art) "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art").ToString
                Elemento.articulo = TablaEncabezado.Rows(i).Item("Des_Art").ToString
                Elemento.cantidad = TablaEncabezado.Rows(i).Item("Cantidad_Articulo")
                Elemento.tipo = TablaEncabezado.Rows(i).Item("tipolente").ToString
                Elemento.factura = TablaEncabezado.Rows(i).Item("factura").ToString
                Elemento.fecha = TablaEncabezado.Rows(i).Item("fechac").ToString
                Elemento.valor = TablaEncabezado.Rows(i).Item("valor").ToString
                Elemento.color = TablaEncabezado.Rows(i).Item("descripcionColor").ToString
                Elemento.vendedor = TablaEncabezado.Rows(i).Item("usuario").ToString
                Elemento.marca = TablaEncabezado.Rows(i).Item("nom_marca").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public descripcion As String
        Public codigo As String
        Public articulo As String
        Public cantidad As String
        Public fecha As String
        Public tipo As String
        Public factura As String
        Public valor As Double
        Public color As String
        Public vendedor As String
        Public marca As String
    End Class


End Class