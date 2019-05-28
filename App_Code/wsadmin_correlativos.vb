Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsadmin_correlativos
    Inherits System.Web.Services.WebService

    'accion para obtener los datos de correlativos 
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of datos)
        Dim SQL As String = "SELECT c.id_correlativo,c.Autorizacion,convert(varchar,c.fecha,103) as fecha ,c.Series,c.Fact_inic,c.Fact_fin,c.Status,c.Corr_Act,c.Caja,c.Doc,c.Establecimiento,e.id_empresa,e.nombre " &
            " FROM  [Correlativos] c INNER JOIN  [ENCA_CIA] e on e.id_empresa = c.id_empresa where c.Status = 1 and e.estado = 1"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_correlativo")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Series").ToString.Trim
                Elemento.empresa = TablaEncabezado.Rows(i).Item("nombre")
                Elemento.idempresa = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.inicio = TablaEncabezado.Rows(i).Item("Fact_inic")
                Elemento.final = TablaEncabezado.Rows(i).Item("Fact_fin")
                Elemento.actual = TablaEncabezado.Rows(i).Item("Corr_Act")
                Elemento.Autorizacion = TablaEncabezado.Rows(i).Item("Autorizacion")
                Elemento.fecha = TablaEncabezado.Rows(i).Item("fecha")
                Elemento.Doc = TablaEncabezado.Rows(i).Item("Doc")
                Elemento.caja = TablaEncabezado.Rows(i).Item("Caja")
                Elemento.establecimiento = TablaEncabezado.Rows(i).Item("Establecimiento")
                Select Case TablaEncabezado.Rows(i).Item("Doc")
                    Case 1
                        Elemento.Doc_desc = "FACTURACION ELECTRONICA"
                    Case 2
                        Elemento.Doc_desc = "NOTA DE CREDITO"
                    Case 3
                        Elemento.Doc_desc = "FACTURACIÓN COPIA"
                    Case 4
                        Elemento.Doc_desc = "NOTA DE ABONO"
                    Case 5
                        Elemento.Doc_desc = "NOTA DE DEBITO"
                    Case 6
                        Elemento.Doc_desc = "NOTA DE DEBITO COPIA"
                    Case 7
                        Elemento.Doc_desc = "NOTA DE ABONO COPIA"
                    Case 8
                        Elemento.Doc_desc = "RECIBO DE CAJA ELECTRONICO"
                    Case 9
                        Elemento.Doc_desc = "RECIBO DE CAJA COPIA"
                    Case 10
                        Elemento.Doc_desc = "NOTA DE CREDITO COPIA"
                    Case Else

                End Select

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'accion para obtener los datos de correlativos por tipo 
    <WebMethod()>
    Public Function ObtenerDatosPorTipoEmpresa(ByVal id_empresa As Integer, ByVal tipo As Integer) As List(Of datos)
        Dim SQL As String = "SELECT [id_correlativo],[Series],[Fact_inic],[Fact_fin] FROM  [Correlativos] where [Status] = 1 and [id_empresa] = " & id_empresa & " and [Doc] = " & tipo

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_correlativo")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Series").ToString & "( " & TablaEncabezado.Rows(i).Item("Fact_inic").ToString & "-" & TablaEncabezado.Rows(i).Item("Fact_fin").ToString & ")"

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function


    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function Insertar(ByVal serie As String, ByVal autorizacion As String, ByVal empresa As String, ByVal inicio As Integer, ByVal final As Integer, ByVal fecha As String, ByVal actual As Integer, ByVal doc As Integer, ByVal establecimiento As Integer, ByVal caja As Integer, ByVal user As String) As String
        'consulta sql

        Dim fechaformat As String() = fecha.Split("/")

        fecha = fechaformat(2) & "-" & fechaformat(1) & "-" & fechaformat(0)
        Dim sql As String = "INSERT INTO  [Correlativos]([Autorizacion],[fecha],[Series],[Fact_inic],[Fact_fin],[Status],[Corr_Act],[Caja],[Doc],[Establecimiento],[id_empresa]) " &
            " VALUES('" & autorizacion & "','" & fecha & "','" & serie.ToString.Trim & "', " & inicio & ", " & final & ",1," & actual & "," & caja & "," & doc & ", " & establecimiento & "," & empresa & " )"
        Dim tipo As String = ""

        Select Case doc
            Case 1
                tipo = "FACTURACION ELECTRONICA"
            Case 2
                tipo = "NOTA DE CREDITO"
            Case 3
                tipo = "FACTURACIÓN COPIA"
            Case 4
                tipo = "NOTA DE ABONO"
            Case 5
                tipo = "NOTA DE DEBITO"
            Case 6
                tipo = "NOTA DE DEBITO COPIA"
            Case 7
                tipo = "NOTA DE ABONO COPIA"
            Case 8
                tipo = "RECIBO DE CAJA ELECTRONICO"
            Case 9
                tipo = "RECIBO DE CAJA COPIA"
            Case 10
                tipo = "NOTA DE CREDITO COPIA"
            Case Else

        End Select



        Dim result As String = ""
        Dim info As String = "CREACION DE NUEVO CORRELATIVO PARA " & tipo & "  CON SERIE: " & serie & ", FECHA DE AUTORIZACION: " & fecha & ", AUTORIZACION: " & autorizacion & ", Establecimiento: " & establecimiento & ", CAJA: " & caja & ", DEL " & inicio & " AL " & final & " Correlativo Actual: " & actual & "."





        Dim sql_log As String = "INSERT INTO  [historial_correlativos] ([informacion],[fecha],[usuario] ,[estado]) VALUES('" & info & "',GETDATE(),'" & user & "',1)"





        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
            manipular.EjecutaTransaccion1(sql_log)
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function


    'Metodo para Actualizar Los datos
    <WebMethod()>
    Public Function Actualizar(ByVal serie As String, ByVal autorizacion As String, ByVal empresa As String, ByVal inicio As Integer, ByVal final As Integer,
                               ByVal fecha As String, ByVal actual As Integer, ByVal doc As Integer, ByVal establecimiento As Integer, ByVal caja As Integer, ByVal id As Integer, ByVal user As String) As String
        'consulta sql

        Dim fechaformat As String() = fecha.Split("/")

        fecha = fechaformat(2) & "-" & fechaformat(1) & "-" & fechaformat(0)

        Dim sql As String = "UPDATE   [Correlativos] SET Autorizacion = '" + autorizacion + "',fecha = '" & fecha & "', Series = '" & serie.ToString.Trim & "',Fact_inic = " & inicio & ",Fact_fin = " & final & ",Corr_Act =" & actual &
        ",Caja =" & caja & ",Doc = " & doc & ",Establecimiento = " & establecimiento & ",id_empresa = " & empresa & " where id_correlativo =  " & id


        Dim result As String = ""

        Dim sql_detalle As String = "SELECT c.id_correlativo,c.Autorizacion,convert(varchar,c.fecha,103) as fecha ,c.Series,c.Fact_inic,c.Fact_fin,c.Status,c.Corr_Act,c.Caja,c.Doc,c.Establecimiento,e.id_empresa,e.nombre " &
            " FROM  [Correlativos] c INNER JOIN  [ENCA_CIA] e on e.id_empresa = c.id_empresa where c.Status = 1 and e.estado = 1 and c.id_correlativo = " & id




        Dim tipo As String = ""
        Select Case doc
            Case 1
                tipo = "FACTURACION ELECTRONICA"
            Case 2
                tipo = "NOTA DE CREDITO"
            Case 3
                tipo = "FACTURACIÓN COPIA"
            Case 4
                tipo = "NOTA DE ABONO"
            Case 5
                tipo = "NOTA DE DEBITO"
            Case 6
                tipo = "NOTA DE DEBITO COPIA"
            Case 7
                tipo = "NOTA DE ABONO COPIA"
            Case 8
                tipo = "RECIBO DE CAJA ELECTRONICO"
            Case 9
                tipo = "RECIBO DE CAJA COPIA"
            Case 10
                tipo = "NOTA DE CREDITO COPIA"
            Case Else

        End Select


        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql_detalle)

        Dim autorizacion_ant As String = ""
        Dim inicio_ant As String = ""
        Dim final_ant As String = ""
        Dim fecha_ant As String = ""
        Dim actual_ant As String = ""
        Dim doc_ant As String = ""
        Dim establecimiento_ant As String = ""
        Dim caja_ant As String = ""
        Dim serie_ant As String = ""

        For i = 0 To TablaEncabezado.Rows.Count - 1
            serie_ant = TablaEncabezado.Rows(i).Item("Series").ToString.Trim
            inicio_ant = TablaEncabezado.Rows(i).Item("Fact_inic").ToString.Trim
            final_ant = TablaEncabezado.Rows(i).Item("Fact_fin").ToString.Trim
            actual_ant = TablaEncabezado.Rows(i).Item("Corr_Act").ToString.Trim
            autorizacion_ant = TablaEncabezado.Rows(i).Item("Autorizacion").ToString.Trim
            fecha_ant = TablaEncabezado.Rows(i).Item("fecha").ToString.Trim
            caja_ant = TablaEncabezado.Rows(i).Item("Caja").ToString.Trim
            establecimiento_ant = TablaEncabezado.Rows(i).Item("Establecimiento").ToString.Trim
            Select Case TablaEncabezado.Rows(i).Item("Doc").ToString.Trim
                Case 1
                    doc_ant = "FACTURACION ELECTRONICA"
                Case 2
                    doc_ant = "NOTA DE CREDITO"
                Case 3
                    doc_ant = "FACTURACIÓN COPIA"
                Case 4
                    doc_ant = "NOTA DE ABONO"
                Case 5
                    doc_ant = "NOTA DE DEBITO"
                Case 6
                    doc_ant = "NOTA DE DEBITO COPIA"
                Case 7
                    doc_ant = "NOTA DE ABONO COPIA"
                Case 8
                    doc_ant = "RECIBO DE CAJA ELECTRONICO"
                Case 9
                    doc_ant = "RECIBO DE CAJA COPIA"
                Case 10
                    doc_ant = "NOTA DE CREDITO COPIA"
                Case Else

            End Select
        Next



        Dim info As String = "ACTUALIZACION DE CORRELATIVO CON SERIE: " & serie_ant & " A " & serie & ", FECHA DE AUTORIZACION: " & fecha_ant & " A " & fecha &
            ", AUTORIZACION: " & autorizacion_ant.Trim() & " A " & autorizacion.Trim() & ", Establecimiento: " & establecimiento_ant & " A " & establecimiento & ", CAJA: " & caja_ant &
            " A " & caja & ", INICIO  " & inicio_ant & " A " & inicio & ", FINAL  " & final_ant & " A  " & final & ", Correlativo Actual: " & actual_ant & " A " & actual & ", TIPO " & doc_ant & " A " & tipo & "."





        Dim sql_log As String = "INSERT INTO  [historial_correlativos] ([informacion],[fecha],[usuario] ,[estado]) VALUES('" & info & "',GETDATE(),'" & user & "',1)"



        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente."
            manipular.EjecutaTransaccion1(sql_log)
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function


    'Metodo para Eliminar Los datos
    <WebMethod()>
    Public Function Inhabilitar(ByVal id As Integer, ByVal usuario As String) As String
        'consulta sql
        Dim sql As String = "UPDATE   [Correlativos] set   Status = 0 where id_correlativo = " & id


        Dim result As String = ""



        Dim sql_detalle As String = "SELECT c.id_correlativo,c.Autorizacion,convert(varchar,c.fecha,103) as fecha ,c.Series,c.Fact_inic,c.Fact_fin,c.Status,c.Corr_Act,c.Caja,c.Doc,c.Establecimiento,e.id_empresa,e.nombre " &
            " FROM  [Correlativos] c INNER JOIN  [ENCA_CIA] e on e.id_empresa = c.id_empresa where c.Status = 1 and e.estado = 1 and c.id_correlativo = " & id


        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql_detalle)

        Dim autorizacion_ant As String = ""
        Dim inicio_ant As String = ""
        Dim final_ant As String = ""
        Dim fecha_ant As String = ""
        Dim actual_ant As String = ""
        Dim doc_ant As String = ""
        Dim establecimiento_ant As String = ""
        Dim caja_ant As String = ""
        Dim serie_ant As String = ""

        For i = 0 To TablaEncabezado.Rows.Count - 1
            serie_ant = TablaEncabezado.Rows(i).Item("Series").ToString.Trim
            inicio_ant = TablaEncabezado.Rows(i).Item("Fact_inic").ToString.Trim
            final_ant = TablaEncabezado.Rows(i).Item("Fact_fin").ToString.Trim
            actual_ant = TablaEncabezado.Rows(i).Item("Corr_Act").ToString.Trim
            autorizacion_ant = TablaEncabezado.Rows(i).Item("Autorizacion").ToString.Trim
            fecha_ant = TablaEncabezado.Rows(i).Item("fecha").ToString.Trim
            caja_ant = TablaEncabezado.Rows(i).Item("Caja").ToString.Trim
            establecimiento_ant = TablaEncabezado.Rows(i).Item("Establecimiento").ToString.Trim
            Select Case TablaEncabezado.Rows(i).Item("Doc").ToString.Trim
                Case 1
                    doc_ant = "FACTURACION ELECTRONICA"
                Case 2
                    doc_ant = "NOTA DE CREDITO"
                Case 3
                    doc_ant = "FACTURACIÓN COPIA"
                Case 4
                    doc_ant = "NOTA DE ABONO"
                Case 5
                    doc_ant = "NOTA DE DEBITO"
                Case 6
                    doc_ant = "NOTA DE DEBITO COPIA"
                Case 7
                    doc_ant = "NOTA DE ABONO COPIA"
                Case 8
                    doc_ant = "RECIBO DE CAJA ELECTRONICO"
                Case 9
                    doc_ant = "RECIBO DE CAJA COPIA"
                Case 10
                    doc_ant = "NOTA DE CREDITO COPIA"
                Case Else

            End Select
        Next




        Dim info As String = "SE DESHABILITO EL  CORRELATIVO DE TIPO  " & doc_ant & "  CON SERIE: " & serie_ant & ", FECHA DE AUTORIZACION: " & fecha_ant & ", AUTORIZACION: " & autorizacion_ant & ", Establecimiento: " & establecimiento_ant & ", CAJA: " & caja_ant & ", DEL " & inicio_ant & " AL " & final_ant & " Correlativo Actual: " & actual_ant & "."




        Dim sql_log As String = "INSERT INTO  [historial_correlativos] ([informacion],[fecha],[usuario] ,[estado]) VALUES('" & info & "',GETDATE(),'" & usuario & "',1)"



        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente"
            manipular.EjecutaTransaccion1(sql_log)
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerHistorial() As List(Of historial)
        Dim SQL As String = "SELECT informacion,convert(varchar, fecha,101) +' '+ convert(varchar, fecha,108) as fecha ,usuario  FROM  historial_correlativos where estado = 1"

        Dim result As List(Of historial) = New List(Of historial)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1

            Dim Elemento As New historial
            Elemento.fecha = TablaEncabezado.Rows(i).Item("fecha")
            Elemento.usuario = TablaEncabezado.Rows(i).Item("usuario")
            Elemento.informacion = TablaEncabezado.Rows(i).Item("informacion")

            result.Add(Elemento)
        Next

        Return result
    End Function


    Public Class historial
        Public fecha As String
        Public usuario As String
        Public informacion As String
    End Class


    'Clase para devolver los datos
    Public Class datos
        Public id As Integer
        Public descripcion As String
        Public empresa As String
        Public idempresa As Integer
        Public inicio As Integer
        Public final As Integer
        Public Autorizacion As String
        Public fecha As String
        Public Doc As Integer
        Public Doc_desc As String
        Public actual As Integer
        Public caja As Integer
        Public establecimiento As Integer
    End Class
End Class