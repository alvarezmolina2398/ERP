Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsvendedores
    Inherits System.Web.Services.WebService

    'metod utilizado para mostrar el listado de los vendedores 
    <WebMethod()> _
    Public Function ObtenerDatos() As List(Of Datos)

        'enviar empresa
        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim StrEncabezado As String = "SELECT * FROM vendedores WHERE status = 1"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_vendedor")
                Elemento.nombre = TablaEncabezado.Rows(i).Item("nombre").ToString
                Elemento.telefono = TablaEncabezado.Rows(i).Item("telefono").ToString
                Elemento.comision = TablaEncabezado.Rows(i).Item("id_comicion")
                Elemento.correo = TablaEncabezado.Rows(i).Item("correo").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para insertar un vendedor
    <WebMethod()> _
    Public Function GuardarDatos(ByVal nombre As String, ByVal comision As Integer, ByVal telefono As Integer, ByVal correo As String) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "INSERT INTO  vendedores (nombre, id_comicion,status, empresa,telefono, correo) VALUES ('" & nombre & "'," & comision & ",1,1," & telefono & ",'" & correo & "')"
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para actualizar los datos de un vendedor
    <WebMethod()> _
    Public Function ActualizarDatos(ByVal nombre As String, ByVal comision As Integer, ByVal telefono As Integer, ByVal correo As String, ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE vendedores SET nombre = '" & nombre & "', id_comicion = " & comision & ", telefono=" & telefono & ", correo='" & correo & "' WHERE id_vendedor=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para deshabilitar un vendedor
    <WebMethod()> _
    Public Function Eliminar(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE vendedores SET status=0 WHERE id_vendedor=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public nombre As String
        Public telefono As String
        Public comision As Integer
        Public correo As String
    End Class

End Class