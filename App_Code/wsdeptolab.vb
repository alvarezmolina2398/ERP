Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsdeptolab
    Inherits System.Web.Services.WebService
    'metod utilizado para mostrar el listado de los vendedores 
    <WebMethod()> _
    Public Function ObtenerDatos() As List(Of Datos)

        'enviar empresa
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT * FROM DEPTO_LAB where estado = 1 "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_departamento")
                Elemento.idmarca = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.nombre = TablaEncabezado.Rows(i).Item("descripcion").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para insertar un vendedor
    <WebMethod()> _
    Public Function GuardarDatos(ByVal empresa As Integer, ByVal nombre As String) As Boolean
        Dim result As Boolean = False
        Dim id As String = Nothing
        id = manipular.idempresabusca("select max(id_departamento) from DEPTO_LAB")
        Dim ide As Double = 0

        If id = "" Then
            ide = 1
        Else
            ide = Convert.ToDouble(id) + 1
        End If
        Dim StrEncabezado As String = "INSERT INTO DEPTO_LAB (id_empresa, id_departamento, descripcion, estado) VALUES (" & empresa & "," & ide & ",'" & nombre & "',1)"
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para actualizar los datos de un vendedor
    <WebMethod()> _
    Public Function ActualizarDatos(ByVal empresa As Integer, ByVal nombre As String, ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE  DEPTO_LAB SET id_empresa = " & empresa & ", descripcion = '" & nombre & "' WHERE id_departamento=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para deshabilitar un vendedor
    <WebMethod()> _
    Public Function Eliminar(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE  DEPTO_LAB SET estado=0 WHERE id_departamento=" & id
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