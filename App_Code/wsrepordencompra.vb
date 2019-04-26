Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsrepordencompra
    Inherits System.Web.Services.WebService

    'consultar detalle 
    <WebMethod()> _
    Public Function consultar(ByVal fechaIni As String, ByVal fechaFin As String, ByVal estatus As Integer) As List(Of [Datos])
        Dim filtro As String = ""
        Dim result As List(Of [Datos]) = New List(Of Datos)()

        If estatus = 0 Then
            filtro = "and eoc.estatus = 0"
        End If

        If estatus = 1 Then
            filtro = "and eoc.estatus = 1"
        End If

        Dim StrEncabezado As String = "SELECT " &
                                      "id_enc, Nom_Pro, convert(varchar(10),eoc.fecha,103) as fecha, ISNULL((SELECT SUM(Sub_Total) FROM DET_OR_COMPRA doc WHERE doc.id_enc = eoc.id_enc), 0) total,eoc.id_consumo,observaciones, " &
                                      "(Select d2.valor from det_metas_sem d2 inner join det_metas d1 on d1.id_presu = d2.id_presu where d1.cuenta =  eoc.id_suc  and d1.periodoa = '2019' and d2.semana = '04' and d1.centroc = 2 and d1.tipo = 'M') totalpres,eoc.Id_suc, " &
                                      "(SELECT sum(total_factura) FROM ENC_COMPRA where month(fecha) = '04' and year(fecha) = '2019') gastado,eoc.estatus " &
                                      "FROM ENC_OR_COMPRA eoc " &
                                      "INNER JOIN PROVEEDOR  p ON eoc.Id_Proveedor = p.Id_PRO " &
                                      "WHERE CAST(eoc.Fecha as date) between '" & fechaIni & "'  and '" & fechaFin & "' " & filtro & " " &
                                      "GROUP BY id_enc, Nom_Pro, eoc.Fecha,eoc.id_consumo,eoc.observaciones,eoc.Id_suc,eoc.estatus "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.noorden = TablaEncabezado.Rows(i).Item("id_enc")
                Elemento.proveedor = TablaEncabezado.Rows(i).Item("Nom_Pro").ToString
                Elemento.fecha = TablaEncabezado.Rows(i).Item("fecha").ToString
                Elemento.valor = Convert.ToDouble(TablaEncabezado.Rows(i).Item("total")).ToString("#,##0.00")
                Elemento.observacion = TablaEncabezado.Rows(i).Item("observaciones").ToString
                If TablaEncabezado.Rows(i).Item("estatus") = 0 Then
                    Elemento.estatus = "Pendiente de Autorizacion"
                ElseIf TablaEncabezado.Rows(i).Item("estatus") = 1 Then
                    Elemento.estatus = "Pendiente de Aprobacion"
                Else
                    Elemento.estatus = "Autorizada"
                End If
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next
        Return result
    End Function

    Public Class Datos
        Public noorden As Integer
        Public proveedor As String
        Public fecha As String
        Public valor As String
        Public observacion As String
        Public estatus As String
    End Class


End Class