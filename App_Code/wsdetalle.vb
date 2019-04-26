Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsdetalle
    Inherits System.Web.Services.WebService
    <WebMethod()> _
    Public Function ObeterDatos() As List(Of [Datos])
        'enviar empresa
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "  select d.id_deta as id, d.descripcion as des, m.id as idm, m.descripcion as desm, d.direccion as dir from DETA_MENU d " &
                                      "   join ENCA_MENU m" &
                                      " on m.id = d.id_menu where d.estado  =1 and m.estado =" & 1
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("des")
                Elemento.idmenu = TablaEncabezado.Rows(i).Item("idm")
                Elemento.menu = TablaEncabezado.Rows(i).Item("desm").ToString
                Elemento.dir = TablaEncabezado.Rows(i).Item("dir").ToString
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next
        Return result
    End Function

    'metodo utilizado para insertar un vendedor
    <WebMethod()> _
    Public Function GuardarDatos(ByVal menu As Integer, ByVal nombre As String, ByVal dir As String) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "INSERT INTO   DETA_MENU (id_menu, descripcion, direccion, estado) VALUES (" & menu & ",'" & nombre & "', '" & dir & "' ,1)"
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para actualizar los datos de un vendedor
    <WebMethod()> _
    Public Function ActualizarDatos(ByVal menu As Integer, ByVal nombre As String, ByVal dir As String, ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE  DETA_MENU SET id_menu = " & menu & ", descripcion = '" & nombre & "', direccion = '" & dir & "' WHERE id_deta=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para deshabilitar un vendedor
    <WebMethod()> _
    Public Function Eliminar(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE  DETA_MENU SET estado=0 WHERE id_deta=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public descripcion As String
        Public idmenu As Integer
        Public menu As String
        Public dir As String
    End Class

End Class