Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wscargar_datos
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function cargarDepartamentos() As List(Of datos)
        Dim SQL As String = "SELECT ID_DEPTO,DESC_DEPTO  FROM  [DEPARTAMENTOS]"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("ID_DEPTO")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("DESC_DEPTO")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function cargarTiposPago() As List(Of datos)
        Dim SQL As String = "SELECT [idtipoPago],[descripcion] FROM  [TipoPago] where [estado] = 1"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idtipoPago")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("descripcion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function cargarTiposTarjetas() As List(Of datos)
        Dim SQL As String = "SELECT  [idTipoTarjeta],[tipoTarjeta] FROM  [TIPOTARJETA] WHERE [estado] = 1"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idTipoTarjeta")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("tipoTarjeta")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function validarProducto(id As Integer) As Integer
        Dim SQL As String = "select COUNT(*) as datos from  Articulo where id_art = '" & id & "'"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        Dim contador As Integer = 0
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                contador = TablaEncabezado.Rows(i).Item("datos")
                ii = ii + 1
            Next
        Next

        Return contador
    End Function

    <WebMethod()>
    Public Function cargarProveedor() As List(Of datos)
        Dim SQL As String = "SELECT [Id_PRO], [nit_pro],[Nom_pro] FROM  [PROVEEDOR]"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_PRO")
                Elemento.extra = TablaEncabezado.Rows(i).Item("nit_pro")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_pro")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function cargarTipoPedido() As List(Of datos)
        Dim SQL As String = "SELECT  [ID_pedido],[DESCRIPCION]  FROM  [TIPO_pedido]"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("ID_pedido")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("DESCRIPCION")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function cargarMarcas() As List(Of datos)
        Dim SQL As String = "SELECT [id_marca],[nom_marca] FROM  [Marcas] WHERE estado = 1"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_marca")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("nom_marca")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function cargarActividades() As List(Of datos)

        Dim result As List(Of datos) = New List(Of datos)()
        Dim StrEncabezado As String = "select id_actividad, descripcion from  ACTIVIDADES_CIA WHERE estado = 1 order by id_actividad"
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            Dim Elemento As New datos
            Elemento.id = TablaEncabezado.Rows(i).Item("id_actividad").ToString()
            Elemento.descripcion = TablaEncabezado.Rows(i).Item("descripcion").ToString()
            result.Add(Elemento)
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function cargarColores() As List(Of datos)
        Dim SQL As String = "SELECT [idColor],[descripcionColor] FROM  [COLOR]"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idColor")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("descripcionColor")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function cargarClasificacionCliente() As List(Of datos)
        Dim SQL As String = "SELECT [id_clasif],[Clasificacion]FROM  [Clasificacion_Cliente]"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_clasif")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Clasificacion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function cargarTipo() As List(Of datos)
        Dim SQL As String = "SELECT  [idTipoLente],[tipoLente] FROM  [TIPOARTICULO] WHERE estado = 1"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idTipoLente")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("tipoLente")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function cargarClasificacion() As List(Of datos)
        Dim SQL As String = "SELECT [Id_Tipo_Art],[Nom_Tipo_Art] FROM  [clasificacionarticulo] where estado = 1"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Tipo_Art")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_Tipo_Art")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function CargarSolicitante(ByVal cia As String) As List(Of datos)
        Dim sql As String = "SELECT [id],[nombre],[correo],[estado] ,[id_dep] FROM [empleado] where estado = 1 and  id_dep = " & cia
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)


        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("nombre")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function cargarDepartamentoLaboral(ByVal id As Integer) As List(Of datos)
        Dim SQL As String = "SELECT  [id_empresa],[id_departamento],[descripcion]  FROM  [DEPTO_LAB] WHERE id_empresa = " & id

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_departamento")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("descripcion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function cargarMunicipiosPorDep(ByVal dep As Integer) As List(Of datos)
        Dim SQL As String = "SELECT ID_MUNI,DESCRIPCION FROM  [MUNICIPIOS] WHERE ID_DEPTO = " & dep

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("ID_MUNI")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("DESCRIPCION")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function cargarSubMarcasporMarc(ByVal marca As Integer) As List(Of datos)
        Dim SQL As String = "SELECT [idSubMarca],[descSubMarca] FROM  [SUB_MARCA] WHERE estado = 1 AND  idMarca = " & marca

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idSubMarca")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("descSubMarca")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function



    <WebMethod()>
    Public Function cargarSubClasificacionAtr(ByVal art As Integer) As List(Of datos)
        Dim SQL As String = "SELECT  [idSubClasiArt],[descSubClasi] FROM  [SUB_CLASIFICACION] WHERE Id_Tipo_Art = " & art

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idSubClasiArt")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("descSubClasi")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    Public Class datos
        Public id As Integer
        Public descripcion As String
        Public extra As String
    End Class

End Class