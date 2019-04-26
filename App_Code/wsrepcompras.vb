Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsrepcompras
    Inherits System.Web.Services.WebService

    'metodo utilizado para llenar las sucursales
    <WebMethod()> _
    Public Function getsuc(ByVal id As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select id_suc, descripcion from SUCURSALES where id_empresa = " & id
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

    'metodo utilizado para obtener los proveedores
    <WebMethod()> _
    Public Function getprov() As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select * from PROVEEDOR"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_PRO")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_Pro").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next


        Return result
    End Function

    'metodo utilizado para obtener el listado de compras 
    <WebMethod()> _
    Public Function consultar(ByVal fechaIn As String, ByVal FechaFin As String, ByVal proveedor As String) As List(Of [Datos])
        Dim filtro = ""

        If proveedor <> "" Then
            filtro = "and p.Nom_pro = '" & proveedor & "'"
        End If



        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select ec.id_enc_orcompra_ing as idc,ec.id_enc_orden,ec.usuario,CONVERT(varchar(10), CAST(ec.fecha as date), 103) as fec,ec.observaciones,ec.idproveedor,p.Nom_Pro, " &
                                    "ISNULL((select sum(cantidad*valor) from det_compra_exterior where id_enc_orcompra_ing = ec.id_enc_orcompra_ing), 0) valor " &
                                    "from enc_compra_exterior ec " &
                                    "inner join Proveedor p on p.Id_PRO = ec.idproveedor  " &
                                    "WHERE  CAST(ec.Fecha as date) between '" & fechaIn & "' and '" & FechaFin & "' " & filtro & "" &
                                    "ORDER BY ec.id_enc_orcompra_ing "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos

                Elemento.noCompra = TablaEncabezado.Rows(i).Item("idc")
                Elemento.fecha = TablaEncabezado.Rows(i).Item("fec").ToString
                Elemento.usuario = TablaEncabezado.Rows(i).Item("usuario").ToString
                Elemento.proveedor = TablaEncabezado.Rows(i).Item("Nom_Pro").ToString
                Elemento.observaciones = TablaEncabezado.Rows(i).Item("observaciones").ToString
                Elemento.valor = Convert.ToDouble(TablaEncabezado.Rows(i).Item("valor")).ToString("#,##0.00")

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'consultar detalle 
    <WebMethod()> _
    Public Function consultarDet(ByVal nocompra As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = " select a.cod_Art, a.Des_Art, dc.cantidad, dc.valor, (dc.cantidad * dc.valor ) total" &
                                      " from DET_COMPRA_exterior dc" &
                                      " join Articulo a" &
                                      " on a.id_art = dc.id_art" &
                                      " where dc.id_enc_orcompra_ing =" & nocompra
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)
        Dim tot As Double = 0
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos

                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art").ToString
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art").ToString
                Elemento.cantidad = TablaEncabezado.Rows(i).Item("cantidad")
                Elemento.precio = Convert.ToDouble(TablaEncabezado.Rows(i).Item("valor").ToString).ToString("#,##0.00")
                Elemento.total = Convert.ToDouble(TablaEncabezado.Rows(i).Item("total").ToString).ToString("#,##0.00")
                tot += Convert.ToDouble(TablaEncabezado.Rows(i).Item("total").ToString)
                Elemento.totalf = tot.ToString("#,##0.00")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    Public Class Datos
        Public id As Integer
        Public descripcion As String
        Public noCompra As Integer
        Public cantidad As Integer
        Public codigo As String
        Public precio As String
        Public total As String
        Public totalf As String
        Public fecha As String
        Public usuario As String
        Public proveedor As String
        Public observaciones As String
        Public valor As String
    End Class


End Class