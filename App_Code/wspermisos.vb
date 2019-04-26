Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wspermisos
    Inherits System.Web.Services.WebService

    'metodo utilizado para obtener la lista de roles
    <WebMethod()> _
    Public Function obtenerRoles() As List(Of [datos])
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim strEncabezado As String = "SELECT * FROM ENC_ROLE WHERE estado = 1"
        Dim tabla As DataTable = manipular.Login(strEncabezado)

        For i = 0 To tabla.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = tabla.Rows(i).Item("idRole")
                Elemento.descripcion = tabla.Rows(i).Item("descripcion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para obtener la lista del menu
    <WebMethod()> _
    Public Function obtenerMenu() As List(Of [datos])
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim strEncabezado As String = "SELECT * FROM ENCA_MENU"
        Dim tabla As DataTable = manipular.Login(strEncabezado)

        For i = 0 To tabla.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = tabla.Rows(i).Item("id")
                Elemento.descripcion = tabla.Rows(i).Item("descripcion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'Metodo utilizado para obtener el listado de permisos no asignados 
    <WebMethod()> _
    Public Function obtenerNoAsignados(ByVal menu As Integer, ByVal rol As Integer) As List(Of [datos])
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim strEncabezado As String = "select * from DETA_MENU e " &
                                       "where not exists(select * from ENCA_MENU m " &
                                       "join DET_ROLE d " &
                                       "on d.id_detalle = e.id_deta " &
                                       "join ENC_ROLE r " &
                                       "on r.idRole = d.idRole " &
                                       "where (d.id_encabezado = " & menu & " and r.idRole =" & rol & ")) and e.id_menu = " & menu
        Dim tabla As DataTable = manipular.Login(strEncabezado)

        For i = 0 To tabla.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = tabla.Rows(i).Item("id_deta")
                Elemento.descripcion = tabla.Rows(i).Item("descripcion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para obtener la lista de permisos asignados
    <WebMethod()> _
    Public Function obtenerAsignados(ByVal menu As Integer, ByVal rol As Integer) As List(Of [datos])
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim strEncabezado As String = "select dm.id_deta as id, dm.descripcion as descripcion from DET_ROLE d  " &
                                      "join DETA_MENU dm " &
                                      "on dm.id_deta = d.id_detalle " &
                                      "join ENCA_MENU m " &
                                      "on m.id = dm.id_menu " &
                                      "join ENC_ROLE r " &
                                      "on r.idRole = d.idRole " &
                                      "where d.id_encabezado = " & menu & " and r.idRole =" & rol
        Dim tabla As DataTable = manipular.Login(strEncabezado)

        For i = 0 To tabla.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = tabla.Rows(i).Item("id")
                Elemento.descripcion = tabla.Rows(i).Item("descripcion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para agregar permisos
    <WebMethod()> _
    Public Function addPermisos(ByVal permiso As List(Of Integer), ByVal rol As Integer, ByVal menu As Integer) As Boolean

        Dim result As Boolean = False

        Dim str As String = ""
        For Each p As Integer In permiso
            str = "INSERT INTO DET_ROLE (idRole, id_encabezado, id_detalle) VALUES (" & rol & ", " & menu & ", " & p & ")"
            result = manipular.EjecutaTransaccion1(str)
        Next

        Return result
    End Function

    'metodo utilizado para quitar los permisos
    <WebMethod()> _
    Public Function delPermisos(ByVal permiso As String, ByVal rol As Integer) As Boolean
        Dim result As Boolean = False
        Dim str As String = ""

        str = "DELETE FROM DET_ROLE where id_detalle in (" & permiso & ") and idRole =" & rol
        result = manipular.EjecutaTransaccion1(str)

        Return result
    End Function

    'metodo utilizado para obtener los permisos asignados por usuario
    <WebMethod()> _
    Public Function obtenerPermisos(ByVal us As String) As List(Of [datos])
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim strEncabezado As String = "select m.id, m.descripcion as menu, dm.descripcion as descripcion, dm.direccion from DET_ROLE d " &
                                    "join DETA_MENU dm " &
                                    "on dm.id_deta = d.id_detalle " &
                                    "join ENCA_MENU m " &
                                    "on m.id = dm.id_menu " &
                                    "join ENC_ROLE r " &
                                    "on r.idRole = d.idRole " &
                                    "join USUARIO  u " &
                                    "on u.idRole  = r.idRole " &
                                    "where u.USUARIO = '" + us + "' " &
                                    "group by m.id, m.descripcion, dm.descripcion, dm.direccion "
        Dim tabla As DataTable = manipular.Login(strEncabezado)

        For i = 0 To tabla.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = tabla.Rows(i).Item("id")
                Elemento.descripcion = tabla.Rows(i).Item("descripcion")
                Elemento.menu = tabla.Rows(i).Item("menu")
                Elemento.direccion = tabla.Rows(i).Item("direccion")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function


    Public Class datos
        Public id As Integer
        Public descripcion As String
        Public direccion As String
        Public idrol As Integer
        Public menu As String
        Public idpermiso As Integer
        Public permiso As String
        Public rol As String
    End Class
End Class