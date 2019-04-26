Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsrepestadoCuenta
    Inherits System.Web.Services.WebService


    'metodo utilizado para obtener el reporte de estado de cuenta del cliente 
    <WebMethod()> _
    Public Function consultar(ByVal fechaIn As String, ByVal FechaFin As String, ByVal idClt As String) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim filtros As String = ""
        Dim strBusqueda As String = Nothing

        If fechaIn.Trim.Length > 0 And FechaFin.Trim.Length > 0 Then
            filtros = "where f_f between '" & fechaIn & "' and '" & FechaFin & "' "
            strBusqueda &= "SELECT 'Acumulado ' + documento Fecha_Op, '' Serie_Fact, '0' Num_Fact, sum(Total_Final_Fact) Total_Final_Fact, 'Saldo Acumulado' text, DATEADD(day, -1, '2015-06-01') f_f, '' mes_nombra, documento FROM (  " &
                              "SELECT * FROM ( " &
                              "SELECT CONVERT(varchar(10), a.Fecha_Fact, 103) as Fecha_Op, a.Serie_Fact, a.Num_Fact, a.Total_Final_Fact, node.text, a.Fecha_Fact as f_f, MONTH(a.Fecha_Fact) as mes_nombra, 'Factura' as documento " &
                              "FROM " &
                              "( " &
                              "SELECT Fecha Fecha_Fact, Serie_Fact, firma Num_Fact, Total_Factura Total_Final_Fact, ef.id_enc " &
                              "FROM ENC_FACTURA ef " &
                              "FULL JOIN DET_FACTURA fd on ef.id_enc = fd.Id_Enc " &
                              "INNER JOIN CLiente c on ef.Id_Clt = c.Id_Clt " &
                              "WHERE c.Id_Clt = " & idClt & " " &
                              "GROUP BY Fecha, Serie_Fact, firma, Total_Factura, ef.id_enc " &
                              ") " &
                              "AS a CROSS APPLY " &
                              "( " &
                              "SELECT 'CAE: ' + ISNULL(Cae, '') Descripcion_Deta " &
                              "FROM ENC_FACTURA ef " &
                              "INNER JOIN CLiente c on ef.Id_Clt = c.Id_Clt " &
                              "WHERE c.Id_Clt = " & idClt & " and ef.id_enc = a.id_enc " &
                              "FOR XML PATH('') " &
                              ") AS node(text) " &
                              ") as pruebas " &
                              "UNION ALL " &
                              "SELECT Fecha_Op, Serie_Fact, Num_Fact, SUM(valor) Total_Final_Fact, text, f_f, mes_nombra, documento FROM ( " &
                              "SELECT CONVERT(varchar(10), ecn.fecha, 103) as Fecha_Op, idSerieNota as Serie_Fact, ecn.firma as Num_Fact, " &
                              "CASE WHEN SUM(devolucion) > 0 THEN (devolucion * df.Precio_Unit_Articulo) - df.descuento ELSE SUM(dnc.descuento) END valor, " &
                              "RTRIM(ecn.observaciones + ' Factura ' + RTRIM(Serie_Fact) + '-' + LTRIM(ef.firma)) as text, ecn.fecha as f_f, MONTH(ecn.fecha) as mes_nombra, 'NC' as documento " &
                              "FROM ENC_NOTA_CREDITO ecn " &
                              "INNER JOIN DET_NOTA_CREDITO dnc on ecn.idNota = dnc.idNota " &
                              "INNER JOIN DET_FACTURA df on dnc.id_detalle = df.Id_detalle " &
                              "INNER JOIN ENC_FACTURA ef on ecn.id_enc = ef.id_enc " &
                              "WHERE Id_Clt = " & idClt & " " &
                              "GROUP BY dnc.devolucion, df.Precio_Unit_Articulo, ecn.fecha, ecn.idSerieNota, ecn.firma, ecn.observaciones, ef.Serie_Fact, ef.firma, df.descuento " &
                              ") sumado " &
                              "GROUP BY Fecha_Op, text, f_f, mes_nombra, documento, Serie_Fact, Num_Fact " &
                              "UNION ALL " &
                              "SELECT * FROM " &
                              "( " &
                              "SELECT CONVERT(varchar(10), a.Fecha_Op, 103) as Fecha_Op, a.Serie_Fact, cast(a.Num_Fact as varchar(max)) Num_Fact, a.Total_Final_Fact, node.text, a.Fecha_Op as f_f, MONTH(a.Fecha_Op) as mes_nombra, 'R' as documento FROM " &
                              "( " &
                              "SELECT '' as Serie_Fact, er.idRecibo as Num_Fact, fecha as Fecha_Op, " &
                              "'' concepto, SUM(valor) as Total_Final_Fact " &
                              "FROM ENC_RECIBO er " &
                              "INNER JOIN DET_RECIBO dt on er.idRecibo = dt.idRecibo " &
                              "WHERE id_clt = " & idClt & " " &
                              "GROUP BY er.idRecibo, fecha " &
                              ") " &
                              "AS a CROSS APPLY " &
                              "( " &
                              "SELECT CAST('Factura ' as varchar) + RTRIM(CAST(ef.Serie_Fact as varchar)) + '-' + CAST(ef.firma as varchar) as Descripcion_Deta " &
                              "FROM ENC_RECIBO enc " &
                              "INNER JOIN DET_RECIBO_FACT deta on enc.idRecibo = deta.idRecibo " &
                              "INNER JOIN ENC_FACTURA ef on deta.id_enc = ef.id_enc " &
                              "WHERE enc.id_clt = " & idClt & "  and enc.idRecibo  = a.Num_Fact FOR XML PATH('') " &
                              ") " &
                              "AS node(text) " &
                              ") " &
                              "as pruebas " &
                              ") as veamos WHERE f_f < '" & fechaIn & "' GROUP BY documento " &
                              "UNION ALL "
        End If

        strBusqueda &= "SELECT * FROM ( " &
                    "SELECT * FROM ( " &
                    "SELECT CONVERT(varchar(10), a.Fecha_Fact, 103) as Fecha_Op, a.Serie_Fact, a.Num_Fact, a.Total_Final_Fact, node.text, a.Fecha_Fact as f_f, MONTH(a.Fecha_Fact) as mes_nombra, 'Factura' as documento " &
                    "FROM " &
                    "( " &
                    "SELECT Fecha Fecha_Fact, Serie_Fact, firma Num_Fact, Total_Factura Total_Final_Fact, ef.id_enc " &
                    "FROM ENC_FACTURA ef " &
                    "FULL JOIN DET_FACTURA fd on ef.id_enc = fd.Id_Enc " &
                    "INNER JOIN CLiente c on ef.Id_Clt = c.Id_Clt " &
                    "WHERE c.Id_Clt = " + idClt + " " &
                    "GROUP BY Fecha, Serie_Fact, firma, Total_Factura, ef.id_enc " &
                    ") " &
                    "AS a CROSS APPLY " &
                    "( " &
                    "SELECT 'CAE: ' + ISNULL(Cae, '') Descripcion_Deta " &
                    "FROM ENC_FACTURA ef " &
                    "INNER JOIN CLiente c on ef.Id_Clt = c.Id_Clt " &
                    "WHERE c.Id_Clt = " + idClt + " and ef.id_enc = a.id_enc " &
                    "FOR XML PATH('') " &
                    ") AS node(text) " &
                    ") as pruebas " &
                    "UNION All " &
                    "SELECT Fecha_Op, Serie_Fact, Num_Fact, SUM(valor) Total_Final_Fact, text, f_f, mes_nombra, documento FROM ( " &
                    "SELECT CONVERT(varchar(10), ecn.fecha, 103) as Fecha_Op, idSerieNota as Serie_Fact, ecn.firma as Num_Fact, " &
                    "CASE WHEN SUM(devolucion) > 0 THEN SUM((devolucion * df.Precio_Unit_Articulo) - df.descuento) ELSE SUM(dnc.descuento) END valor, " &
                    "RTRIM(ecn.observaciones + ' Factura ' + RTRIM(Serie_Fact) + '-' + LTRIM(ef.firma)) as text, ecn.fecha as f_f, MONTH(ecn.fecha) as mes_nombra, 'NC' as documento " &
                    "FROM ENC_NOTA_CREDITO ecn " &
                    "INNER JOIN DET_NOTA_CREDITO dnc on ecn.idNota = dnc.idNota " &
                    "INNER JOIN DET_FACTURA df on dnc.id_detalle = df.Id_detalle " &
                    "INNER JOIN ENC_FACTURA ef on ecn.id_enc = ef.id_enc " &
                    "WHERE Id_Clt = " + idClt + " " &
                    "GROUP BY dnc.devolucion, df.Precio_Unit_Articulo, ecn.fecha, ecn.idSerieNota, ecn.firma, ecn.observaciones, ef.Serie_Fact, ef.firma, df.descuento " &
                    ") sumado " &
                    "GROUP BY Fecha_Op, text, f_f, mes_nombra, documento, Serie_Fact, Num_Fact " &
                    "UNION ALL " &
                    "SELECT * FROM " &
                    "( " &
                    "SELECT CONVERT(varchar(10), a.Fecha_Op, 103) as Fecha_Op, a.Serie_Fact, cast(a.Num_Fact as varchar(max)) Num_Fact, a.Total_Final_Fact, node.text, a.Fecha_Op as f_f, MONTH(a.Fecha_Op) as mes_nombra, 'R' as documento FROM " &
                    "( " &
                    "SELECT '' as Serie_Fact, er.idRecibo as Num_Fact, fecha as Fecha_Op, " &
                    "'' concepto, SUM(valor) as Total_Final_Fact " &
                    "FROM ENC_RECIBO er " &
                    "INNER JOIN DET_RECIBO dt on er.idRecibo = dt.idRecibo " &
                    "WHERE id_clt = " + idClt + " " &
                    "GROUP BY er.idRecibo, fecha " &
                    ") " &
                    "AS a CROSS APPLY " &
                    "( " &
                    "SELECT CAST('Factura ' as varchar) + RTRIM(CAST(ef.Serie_Fact as varchar)) + '-' + CAST(ef.firma as varchar) as Descripcion_Deta " &
                    "FROM ENC_RECIBO enc " &
                    "INNER JOIN DET_RECIBO_FACT deta on enc.idRecibo = deta.idRecibo " &
                    "INNER JOIN ENC_FACTURA ef on deta.id_enc = ef.id_enc " &
                    "WHERE enc.id_clt = " + idClt + "  and enc.idRecibo  = a.Num_Fact FOR XML PATH('') " &
                    ") " &
                    "AS node(text) " &
                    ") " &
                    "as pruebas " &
                    ") as veamos " + filtros + " order by f_f"

        Dim Busqueda As DataTable = manipular.Llena_Drop(strBusqueda)

        Dim mes_nombrar As Integer = 0
        Dim suma1 As Double = 0
        Dim suma2 As Double = 0
        Dim saldo_mes As Double = 0
        Dim linea As Integer = 0
        Dim prueba_l As Integer = Busqueda.Rows.Count
        Dim quita_lineas As Integer = 0
        Dim saldoacumulativo As Double = 0
        Dim terminoAcumulativo As Integer = 0
        Dim tabla_lista As New DataTable()

        Dim dc1 As New DataColumn
        dc1.ColumnName = "fecha"
        tabla_lista.Columns.Add(dc1)
        Dim dc2 As New DataColumn
        dc2.ColumnName = "concepto"
        tabla_lista.Columns.Add(dc2)
        Dim dc3 As New DataColumn
        dc3.ColumnName = "factura"
        tabla_lista.Columns.Add(dc3)
        Dim dc4 As New DataColumn
        dc4.ColumnName = "nc"
        tabla_lista.Columns.Add(dc4)
        Dim dc5 As New DataColumn
        dc5.ColumnName = "debe"
        tabla_lista.Columns.Add(dc5)
        Dim dc6 As New DataColumn
        dc6.ColumnName = "haber"
        tabla_lista.Columns.Add(dc6)
        Dim dc7 As New DataColumn
        dc7.ColumnName = "saldom"
        tabla_lista.Columns.Add(dc7)
        Dim dc8 As New DataColumn
        dc8.ColumnName = "acumulado"
        tabla_lista.Columns.Add(dc8)

        If Busqueda.Rows.Count > 0 Then

            For Each row As DataRow In Busqueda.Rows

                If row("mes_nombra").ToString = "0" Then
                Else
                    terminoAcumulativo = terminoAcumulativo + 1
                End If

                If mes_nombrar <> row("mes_nombra").ToString Then

                    Dim nombre_mes As String = Nothing

                    If row("mes_nombra").ToString = "0" Then
                        nombre_mes = ""
                    Else
                        nombre_mes = MonthName(row("mes_nombra").ToString)
                        nombre_mes = UCase(Mid(nombre_mes, 1, 1)) + Mid(nombre_mes, 2, Len(nombre_mes))
                    End If

                    If terminoAcumulativo = 1 Then
                        tabla_lista.Rows.Add("ACUMULADO", "SALDO ACUMULADO", "", "", "", "", FormatNumber(suma2, 2), FormatNumber(suma2, 2))
                    End If

                    tabla_lista.Rows.Add("", nombre_mes, "", "", "", "", "", "")

                    If linea > 1 Then
                        Dim linea_cambia As Integer = linea
                        tabla_lista.Rows(linea_cambia).Item("acumulado") = FormatNumber(saldo_mes + suma2, 2)
                        saldo_mes = tabla_lista.Rows(linea_cambia).Item(7)
                        suma2 = 0
                        suma1 = 0
                    End If

                    mes_nombrar = row("mes_nombra").ToString
                    If row("mes_nombra").ToString = "0" Then
                    Else
                        linea = linea + 1
                        quita_lineas = quita_lineas + 1
                    End If

                End If

                Dim debe As String = ""
                Dim haber As String = ""
                Dim recibo As String = ""
                Dim factura As String = ""
                Dim descripciones As String = row("text").ToString.Replace("</Descripcion_Deta>", "").Replace("<Descripcion_Deta>", "").ToString

                If row("mes_nombra").ToString = "0" Then

                    If row("documento").ToString = "NC" Or row("documento").ToString = "NA" Then
                        suma2 = suma2 - Val(row("Total_Final_Fact").ToString)
                    ElseIf row("documento").ToString = "Factura" Then
                        suma2 = suma2 + Val(row("Total_Final_Fact").ToString)
                    ElseIf row("documento").ToString = "R" Then
                        suma2 = suma2 - Val(row("Total_Final_Fact").ToString)
                    End If
                Else
                    If row("documento").ToString = "NC" Or row("documento").ToString = "NA" Then
                        debe = ""
                        haber = FormatNumber(row("Total_Final_Fact").ToString, 2)
                        factura = ""
                        recibo = row("Serie_Fact").ToString & "-" & row("Num_Fact").ToString
                        suma1 = suma1 - row("Total_Final_Fact").ToString
                        suma2 = suma2 - row("Total_Final_Fact").ToString
                        descripciones = "Nota de credito"
                    ElseIf row("documento").ToString = "Factura" Then
                        debe = FormatNumber(row("Total_Final_Fact").ToString, 2)
                        haber = ""
                        factura = row("Serie_Fact").ToString & "-" & row("Num_Fact").ToString
                        recibo = ""
                        suma1 = suma1 + row("Total_Final_Fact").ToString
                        suma2 = suma2 + row("Total_Final_Fact").ToString
                        descripciones = "Compra de equipo"

                    ElseIf row("documento").ToString = "R" Then
                        Dim serie As String = Nothing

                        If row("Serie_Fact").ToString.Trim.Length < 1 Then
                            serie = " is null "
                        Else
                            serie = " = '" & row("Serie_Fact").ToString & "'"
                        End If

                        debe = ""
                        haber = FormatNumber(row("Total_Final_Fact").ToString, 2)
                        factura = ""
                        recibo = row("Serie_Fact").ToString & "IN-" & row("Num_Fact").ToString
                        suma1 = suma1 - row("Total_Final_Fact").ToString
                        suma2 = suma2 - row("Total_Final_Fact").ToString
                        descripciones = "Pago de cliente"

                    End If

                    If (linea - quita_lineas) = Busqueda.Rows.Count - 1 Then
                        tabla_lista.Rows.Add(row("Fecha_Op").ToString, String.Format("<b>{0}</b>", descripciones), factura, recibo, debe, haber, FormatNumber(suma1, 2), FormatNumber(saldo_mes + suma2, 2))
                    Else
                        tabla_lista.Rows.Add(row("Fecha_Op").ToString, String.Format("<b>{0}</b>", descripciones), factura, recibo, debe, haber, FormatNumber(suma1, 2), "")
                    End If
                    If row("mes_nombra").ToString <> "0" Then
                        linea = linea + 1
                    End If
                End If

            Next

            Dim linea_cambiau As Integer = linea
            If saldo_mes <> 0 & suma2 <> 0 Then
                tabla_lista.Rows(linea_cambiau).Item("acumulado") = FormatNumber(saldo_mes + suma2, 2)
            Else
                tabla_lista.Rows(linea_cambiau).Item("acumulado") = 0
            End If

            saldo_mes = tabla_lista.Rows(linea_cambiau).Item(7)
            suma2 = 0
            suma1 = 0

            For Each row2 As DataRow In tabla_lista.Rows
                Dim Elemento As New Datos
                Elemento.fecha = row2(0).ToString
                Elemento.concepto = row2(1).ToString
                Elemento.factura = row2(2).ToString
                Elemento.recibo = row2(3).ToString
                If row2(4).ToString.Length > 0 Then
                    Elemento.debe = row2(4).ToString
                Else
                    Elemento.debe = row2(4).ToString()
                End If
                If row2(5).ToString.Length > 0 Then
                    Elemento.haber = row2(5).ToString
                Else
                    Elemento.haber = row2(5).ToString()
                End If
                If row2(6).ToString.Length > 0 Then
                    Elemento.saldoM = row2(6).ToString
                Else
                    Elemento.saldoM = row2(6).ToString()
                End If
                If row2(7).ToString.Length > 0 Then
                    Elemento.saldoA = row2(7).ToString
                Else
                    Elemento.saldoA = row2(7).ToString
                End If
                result.Add(Elemento)
            Next
        End If

        Return result
    End Function

    'declaracion de clase contenedora de variables a utilizar
    Public Class Datos
        Public concepto As String
        Public recibo As String
        Public fecha As String
        Public factura As String
        Public mes As Integer
        Public debe As String
        Public haber As String
        Public saldoM As String
        Public saldoA As String
    End Class
End Class