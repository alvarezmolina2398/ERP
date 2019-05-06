Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wssubclasificacion
    Inherits System.Web.Services.WebService
    'metod utilizado para mostrar el listado de los vendedores 
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of Datos)

        'enviar empresa
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT s.Id_Tipo_Art, s.idSubClasiArt, c.Des_Tipo_Art , s.descSubClasi FROM SUB_CLASIFICACION s " &
                                    "JOIN clasificacionarticulo c " &
                                    "on c.Id_Tipo_Art = s.Id_Tipo_Art " &
                                    "where s.estado = 1"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idSubClasiArt")
                Elemento.idmarca = TablaEncabezado.Rows(i).Item("Id_Tipo_Art")
                Elemento.marca = TablaEncabezado.Rows(i).Item("Des_Tipo_Art").ToString
                Elemento.nombre = TablaEncabezado.Rows(i).Item("descSubClasi").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para insertar un vendedor
    <WebMethod()>
    Public Function GuardarDatos(ByVal clasif As Integer, ByVal desc As String) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "INSERT INTO   SUB_CLASIFICACION (Id_Tipo_Art, descSubClasi, estado) VALUES (" & clasif & ",'" & desc & "',1)"
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para actualizar los datos de un vendedor
    <WebMethod()>
    Public Function ActualizarDatos(ByVal clasif As Integer, ByVal desc As String, ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE  SUB_CLASIFICACION SET Id_Tipo_Art = " & clasif & ", descSubClasi = '" & desc & "' WHERE idSubClasiArt=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para deshabilitar un vendedor
    <WebMethod()>
    Public Function Eliminar(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE  SUB_CLASIFICACION SET estado=0 WHERE idSubClasiArt=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public nombre As String
        Public idmarca As Integer
        Public marca As String
    End Class

End Class