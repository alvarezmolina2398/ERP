Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wslista_facturas
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function ObtenerFacturas(ByVal fechainicio As String, ByVal fechafin As String) As List(Of datos)

        Dim fechaformat As String() = fechainicio.Split("/")
        fechainicio = fechaformat(2) & "-" & fechaformat(1) & "-" & fechaformat(0)

        fechaformat = fechafin.Split("/")
        fechafin = fechaformat(2) & "-" & fechaformat(1) & "-" & fechaformat(0)


        Dim SQL As String = "SELECT F.id_enc,F.Serie_Fact,convert(varchar,F.Fecha,103)+' '+convert(varchar,F.Fecha,24) as fecha,F.firma,F.Total_Factura, isnull(F.Total_Descuento,0) as Total_Descuento,C.Id_Clt, C.nit_clt, c.Nom_clt " &
            "FROM ENC_FACTURA F " &
            "INNER JOIN CLiente C  ON C.Id_Clt = F.Id_Clt where F.estado = 1 and f.Fecha between  '" & fechainicio & "  00:00:00'  and '" & fechafin & "  23:59:59'"

        Dim result As List(Of datos) = New List(Of datos)
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            Dim Elemento As New datos
            Elemento.id = TablaEncabezado.Rows(i).Item("id_enc")
            Elemento.factura = TablaEncabezado.Rows(i).Item("Serie_Fact") & "-" & TablaEncabezado.Rows(i).Item("firma")
            Elemento.nit = TablaEncabezado.Rows(i).Item("nit_clt")
            Elemento.cliete = TablaEncabezado.Rows(i).Item("Nom_clt")
            Elemento.fecha = TablaEncabezado.Rows(i).Item("fecha")
            Elemento.total = Format(TablaEncabezado.Rows(i).Item("Total_Factura"), "###,##0.00")
            result.Add(Elemento)
        Next
        Return result
    End Function



    <WebMethod()>
    Public Function AnularFactura(ByVal id As Integer, ByVal observacion As String, ByVal usuario As String) As String
        'consulta sql
        Dim sql As String = "INSERT INTO HISTORIAL_ANULARFACTURAS([id_enc],[observacion],[fecha],[usuario],[tipo])VALUES(" & id & ",'" & observacion & "',GETDATE(),'" & usuario & "',1); " &
            "UPDATE   [ENC_FACTURA] SET [estado] = 0 WHERE  id_enc = " & id


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Factura Anulada  Correctamente"
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function



    <WebMethod()>
    Public Function RealizarNotaCredito(ByVal id As Integer, ByVal observacion As String, ByVal usuario As String) As String
        'consulta sql
        Dim sql As String = "INSERT INTO HISTORIAL_ANULARFACTURAS([id_enc],[observacion],[fecha],[usuario],[tipo])VALUES(" & id & ",'" & observacion & "',GETDATE(),'" & usuario & "',2); "



        Dim result As String = ""


        If notacredito() Then
            'ejecuta el query a travez de la clase manipular 
            If (manipular.EjecutaTransaccion1(sql)) Then
                result = "SUCCESS|Factura Anulada  Correctamente"
            Else
                result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
            End If
        Else
            result = "ERROR|Sucedio Un error, NOTA DE CREDITO "
        End If





        Return result
    End Function


    Public Function notacredito() As Boolean
        Return True
    End Function

    Public Class datos
        Public id As Integer
        Public fecha As String
        Public factura As String
        Public nit As String
        Public cliete As String
        Public total As String
    End Class

End Class