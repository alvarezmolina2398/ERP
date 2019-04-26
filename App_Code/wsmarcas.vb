Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsmarcas
    Inherits System.Web.Services.WebService
    'metod utilizado para mostrar el listado de los vendedores 
    <WebMethod()> _
    Public Function ObtenerDatos() As List(Of Datos)

        'enviar empresa
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT * FROM Marcas WHERE estado = 1 "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_marca")
                Elemento.nombre = TablaEncabezado.Rows(i).Item("nom_marca").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para insertar un vendedor
    <WebMethod()> _
    Public Function GuardarDatos(ByVal nombre As String) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "INSERT INTO  Marcas (nom_marca,estado) VALUES ('" & nombre & "',1)"
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para actualizar los datos de un vendedor
    <WebMethod()> _
    Public Function ActualizarDatos(ByVal nombre As String, ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE Marcas SET nom_marca = '" & nombre & "' WHERE id_marca=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para deshabilitar un vendedor
    <WebMethod()> _
    Public Function Eliminar(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE Marcas SET estado=0 WHERE id_marca=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public nombre As String
    End Class

End Class