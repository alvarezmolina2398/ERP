Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsusuarios
    Inherits System.Web.Services.WebService

    'metodo utilizado para mostrar el listado de departamentos laborales
    <WebMethod()> _
    Public Function obtenerSuc(ByVal emp As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select * FROM SUCURSALES WHERE id_empresa = " & emp
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.sucursal = TablaEncabezado.Rows(i).Item("descripcion").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para mostrar el listado de departamentos laborales
    <WebMethod()> _
    Public Function obtenerDeptos() As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select * FROM DEPTO_LAB "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_departamento")
                Elemento.departamento = TablaEncabezado.Rows(i).Item("descripcion").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metod utilizado para mostrar el listado de los vendedores 
    <WebMethod()> _
    Public Function ObtenerDatos() As List(Of Datos)

        'enviar empresa
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select * , s.descripcion as sucursal, r.descripcion as rol  from USUARIO u " &
                                      "join SUCURSALES s " &
                                      "on u.id_sucursal = s.id_suc " &
                                      "JOIN ENC_ROLE r  " &
                                      "on r.idRole = u.idRole  " &
                                      "where u.ESTADO = 1 AND u.id_empresa = 1 "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("ID")
                Elemento.usuario = TablaEncabezado.Rows(i).Item("USUARIO").ToString
                Elemento.empresa = "Empresa demo"
                Elemento.idempresa = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.idtipo = TablaEncabezado.Rows(i).Item("tipo_usuario").ToString
                Elemento.idsucursal = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.sucursal = TablaEncabezado.Rows(i).Item("sucursal")
                Elemento.idrol = TablaEncabezado.Rows(i).Item("idRole")
                Elemento.rol = TablaEncabezado.Rows(i).Item("rol").ToString
                Elemento.nombre = TablaEncabezado.Rows(i).Item("Nombres").ToString
                Elemento.apellido = TablaEncabezado.Rows(i).Item("Apellidos").ToString
                Elemento.password = TablaEncabezado.Rows(i).Item("PASSWORD").ToString
                Elemento.iddepto = TablaEncabezado.Rows(i).Item("id_depto_laboral")
                Elemento.correo = TablaEncabezado.Rows(i).Item("correo").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para insertar un vendedor
    <WebMethod()> _
    Public Function GuardarDatos(ByVal usuario As String, ByVal nombre As String, ByVal apellidos As String, ByVal password As String, ByVal empresa As Integer, ByVal sucursal As Integer, ByVal tipo As String, ByVal depto As Integer, ByVal rol As Integer, ByVal correo As String) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "INSERT INTO  USUARIO (USUARIO, PASSWORD, ESTADO, tipo_usuario, id_empresa, id_sucursal, idRole, id_depto_laboral, Nombres, Apellidos, correo) VALUES ('" & usuario & "', '" & password & "',1,'" & tipo & "', " & empresa & ", " & sucursal & "," & rol & "," & depto & ",'" & nombre & "', '" & apellidos & "', '" & correo & "' )"
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para actualizar los datos de un vendedor
    <WebMethod()> _
    Public Function ActualizarDatos(ByVal usuario As String, ByVal nombre As String, ByVal apellidos As String, ByVal password As String, ByVal empresa As Integer, ByVal sucursal As Integer, ByVal tipo As String, ByVal depto As Integer, ByVal rol As Integer, ByVal id As Integer, ByVal correo As String) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE USUARIO SET USUARIO = '" & usuario & "', PASSWORD = '" & password & "', tipo_usuario = '" & tipo & "', id_empresa = " & empresa & ", id_sucursal = " & sucursal & ", idRole = " & rol & ", id_depto_laboral = " & depto & ", Nombres = '" & nombre & "', Apellidos = '" & apellidos & "', correo = '" & correo & "' WHERE ID = " & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para deshabilitar un vendedor
    <WebMethod()> _
    Public Function Eliminar(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE USUARIO SET estado=0 WHERE ID=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para consultar la empresa
    <WebMethod()> _
    Public Function consultarEmpresa() As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)

        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public nombre As String
        Public usuario As String
        Public apellido As String
        Public password As String
        Public empresa As String
        Public idempresa As Integer
        Public idtipo As String
        Public tipoUsuario As String
        Public sucursal As String
        Public idsucursal As Integer
        Public idrol As Integer
        Public iddepto As Integer
        Public departamento As String
        Public rol As String
        Public correo As String
    End Class

End Class