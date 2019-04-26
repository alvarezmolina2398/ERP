Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsadmin_articulos
    Inherits System.Web.Services.WebService

    'accion para obtener las bodegas
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of datos)
        Dim SQL As String = "SELECT a.id_art,a.cod_Art,a.Des_Art,a.id_tipo,a.id_marca,a.id_clasi,a.costo_art,isnull(a.precio1,0.00) precio1, isnull(a.cod_pro2,' ') cod_pro2, isnull(a.cod_pro1,' ') cod_pro1, " &
            "a.idColor,isnull(a.precio1Es,0.00) precio1Es,isnull(a.id_SubMarca,0) id_SubMarca " &
            ",isnull(a.id_Subclasi,0) id_Subclasi, c.descripcionColor" &
            " FROM [dbo].[Articulo] a " &
            " INNER JOIN [dbo].[COLOR] c on c.idColor = a.idColor where a.estado = 1 "

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_art")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art")
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art")
                Elemento.tipo = TablaEncabezado.Rows(i).Item("id_tipo")
                Elemento.idmarca = TablaEncabezado.Rows(i).Item("id_marca")
                Elemento.idclasificacion = TablaEncabezado.Rows(i).Item("id_clasi")
                Elemento.costo = TablaEncabezado.Rows(i).Item("costo_art")
                Elemento.precioGt = TablaEncabezado.Rows(i).Item("precio1")
                Elemento.precioEs = TablaEncabezado.Rows(i).Item("precio1Es")
                Elemento.codigo1 = TablaEncabezado.Rows(i).Item("cod_pro1")
                Elemento.codigo2 = TablaEncabezado.Rows(i).Item("cod_pro2")
                Elemento.idcolor = TablaEncabezado.Rows(i).Item("idColor")
                Elemento.idsubmarca = TablaEncabezado.Rows(i).Item("id_SubMarca")
                Elemento.id_Subclasificacion = TablaEncabezado.Rows(i).Item("id_Subclasi")
                Elemento.color = TablaEncabezado.Rows(i).Item("descripcionColor")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function


    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function Insertar(ByVal descripcion As String, ByVal codigo As String, ByVal tipo As Integer, ByVal cod_pro1 As String, ByVal cod_pro2 As String, ByVal marca As Integer,
                             ByVal idsubmarca As Integer, ByVal id_clasi As Integer, ByVal id_subclasi As Integer, ByVal idcolor As Integer, ByVal preciogt As Double, ByVal precioEs As Double,
                             ByVal costo As Double, ByVal usuario As String) As String
        'consulta sql
        Dim sql As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[Articulo] (cod_Art,Des_Art,id_tipo,cod_pro1,cod_pro2,id_marca,id_SubMarca,id_clasi,id_Subclasi,idColor,precio1,precio1Es,costo_art,usuario,Estado)  " &
            "VALUES('" & codigo & "', '" & descripcion & "'," & tipo & ",'" & cod_pro1 & "','" & cod_pro2 & "'," & marca & "," & idsubmarca & "," & id_clasi & "," & id_subclasi & "," & idcolor &
            "," & preciogt & "," & precioEs & ", " & costo & ", '" & usuario & "',1);"


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function



    'Metodo para Actualizar Los datos
    <WebMethod()>
    Public Function Actualizar(ByVal descripcion As String, ByVal codigo As String, ByVal tipo As Integer, ByVal cod_pro1 As String, ByVal cod_pro2 As String, ByVal marca As Integer,
                             ByVal idsubmarca As Integer, ByVal id_clasi As Integer, ByVal id_subclasi As Integer, ByVal idcolor As Integer, ByVal preciogt As Double, ByVal precioEs As Double,
                             ByVal costo As Double, ByVal usuario As String, ByVal id As Integer) As String
        'consulta sql
        Dim sql As String = "UPDATE [ERPDEVLYNGT].[dbo].[Articulo] set cod_Art = '" & codigo & "',Des_Art = '" & descripcion & "',id_tipo = " & tipo & ",cod_pro1 = '" & cod_pro1 & "',cod_pro2 = '" & cod_pro2 &
            "',id_marca = " & marca & ",id_SubMarca = " & idsubmarca & ",id_clasi = " & id_clasi & ",id_Subclasi = " & idsubmarca & ",idColor = " & idcolor & ",precio1 = " & preciogt & ",precio1Es=" & precioEs &
            ",costo_art = " & costo & ", usuario = '" & usuario & "' where id_art = " & id



        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente"
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function


    'Metodo para Eliminar Los datos
    <WebMethod()>
    Public Function Inhabilitar(ByVal id As Integer) As String
        'consulta sql
        Dim sql As String = "UPDATE [ERPDEVLYNGT].[dbo].[Articulo] set estado = 0 where id_art = " & id


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente"
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function

    Public Class datos
        Public id As Integer
        Public codigo As String
        Public descripcion As String
        Public tipo As Integer
        Public idmarca As Integer
        Public idclasificacion As Integer
        Public precioGt As Double
        Public costo As Double
        Public codigo1 As String
        Public codigo2 As String
        Public idcolor As Integer
        Public precioEs As Integer
        Public color As String
        Public idsubmarca As Integer
        Public id_Subclasificacion As Integer
    End Class

End Class