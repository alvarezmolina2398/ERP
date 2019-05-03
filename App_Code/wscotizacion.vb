Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports iTextSharp.text
Imports iTextSharp.text.pdf

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wscotizacion
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Cotizar(ByVal usuario As String, ByVal total As Double, ByVal descuento As Double, ByVal idcliente As Integer,
                             ByVal diascredito As Integer, ByVal listproductos As List(Of productos)) As String
        Dim result As String

        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion

        Try

            Dim serie As String = "FAC"
            Dim eliminaDoc1 As String = "FACE66"
            Dim eliminaDoc2 As String = "FACE63"
            Dim eliminaDoc3 As String = serie & "001"
            Dim retornoData As String() = Factura_ElectronicaDemo(serie).Split("|")
            Dim firma As String = retornoData(0)
            Dim cae = retornoData(1)


            total = total - descuento

            Dim totalsiniva As Double = total / 1.12
            Dim iva As Double = totalsiniva * 0.12

            Dim empresa As String = "SELECT id_empresa  FROM [ERPDEVLYNGT].[dbo].[USUARIO] where USUARIO = '" & usuario & "'"
            Dim sucursal As String = "SELECT id_sucursal  FROM [ERPDEVLYNGT].[dbo].[USUARIO] where USUARIO = '" & usuario & "'"

            'INSERCION DE LA FACTURA
            Dim str1 As String = "INSERT INTO [dbo].[ENC_COTIZACION]([USUARIO] ,[id_empresa],[Serie_Fact],[Fecha],[firma] ,[Cae],[Total_Factura],[Iva_Factura],[Total_sin_iva],[Total_Descuento],[Id_Clt],[dias_cred],[id_suc]) " &
                 "VALUES('" & usuario & "', (" & empresa & "),'" & serie & "',GETDATE(),'" & firma & "','" & cae & "'," & total & "," & Math.Round(iva, 2) & "," & Math.Round(totalsiniva, 2) & "," & descuento & "," & idcliente & "," & diascredito & ", (" & sucursal & "));"


            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()



            'INSERCION DEL DETALLE DE LA FACTURA
            For Each item As productos In listproductos

                Dim totalsinivaDesc As Double = (item.cantidad * item.precio) / 1.12
                Dim ivadesc As Double = totalsinivaDesc * 0.12

                Dim str2 As String = "INSERT INTO [dbo].[DET_COTIZACION] ([id_enc],[Cantidad_Articulo],[Precio_Unit_Articulo],[Sub_Total],[Descuento],[Iva],[Total_Sin_Iva],[Total],[Id_Art],[costoPromedio],[Id_Bod]) " &
                    "VALUES(" & id & "," & item.cantidad & "," & item.precio & "," & (item.cantidad * item.precio) & ",0.00," & Math.Round(ivadesc, 2) & "," & Math.Round(totalsinivaDesc, 2) & "," & Math.Round((item.cantidad * item.precio), 2) & "," & item.id & ", ROUND((select CONVERT(varchar,costo_art) from [ERPDEVLYNGT].[dbo].[Articulo] where id_art = " & item.id & "),2)," & item.bodega & ");"

                comando.CommandText = str2
                comando.ExecuteNonQuery()
            Next
            transaccion.Commit()

            Dim id2 As Integer = id
            result = "SUCCESS| COTIZACION GENERADA EXITOSAMENTE|" & CrearPDF(id2, ObtenerSucursal(usuario), usuario, idcliente, "COTIZACION")




        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result
    End Function



    Public Function Factura_ElectronicaDemo(ByVal serie As String) As String

        Dim chars As String = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789#/="
        Dim unit As String = ""
        Dim r As New Random
        Dim fechaHora As DateTime = DateTime.Now

        For i As Integer = 1 To 64

            Dim siguiente As Integer = r.Next(0, chars.Length)
            unit &= chars.Substring(siguiente, 1)

        Next

        Dim siguienteTurno As String = ObtenerSiguienteCorrelativo(serie)

        Dim firma As String = "FACE63" & serie & siguienteTurno
        Dim cae As String = unit

        Return firma & "|" & cae

    End Function


    <WebMethod()>
    Public Function ObtenerSiguienteCorrelativo(ByVal serie As String) As String
        Dim SQL As String = "SELECT ISNULL(MAX(CAST(SUBSTRING(firma, 10, 25) As numeric)), 180000000000) + 1 As Siguiente FROM [ERPDEVLYNGT].[dbo].[ENC_FACTURA] WHERE Serie_Fact = '" & serie & "';"

        Dim result As String = ""
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            result = TablaEncabezado.Rows(i).Item("Siguiente").ToString
        Next

        Return result

    End Function


    <WebMethod()>
    Public Function CrearPDF(ByVal cotizacion As Integer, ByVal idsuc As Integer, ByVal usuario As String, ByVal idcliente As Integer, ByVal observacion As String) As String
        Dim result As String = ""
        Try

            Dim doc As Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 5, 5, 5, 5)
            Dim datafecha As Date = Now
            Dim nombredoc As String = "pdf\orden" & cotizacion & ".pdf"
            Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(Server.MapPath("~\vista\" & nombredoc), FileMode.Create))
            result = nombredoc
            doc.AddTitle("COIZACIÓN # " & cotizacion)
            doc.AddAuthor("")
            doc.AddCreationDate()

            doc.Open()

            Dim PARRAFO_ESPACIO As Paragraph = New Paragraph(" ", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD))
            PARRAFO_ESPACIO.Alignment = Element.ALIGN_CENTER

            Dim d As datos = obtenerDatosEmpresa(idsuc)

            doc.Add(PARRAFO_ESPACIO)
            doc.Add(PARRAFO_ESPACIO)
            doc.Add(PARRAFO_ESPACIO)



            Dim RutaImagen As String = Server.MapPath("~")
            Dim Imagen As Image = Image.GetInstance(RutaImagen & "\img\logo.png")

            Imagen.ScalePercent(30) 'escala al tamaño de la imagen
            Imagen.SetAbsolutePosition(25, 690)

            doc.Add(Imagen)

            Dim Tabla As PdfPTable = New PdfPTable(3)

            Tabla.TotalWidth = 550.0F
            Tabla.LockedWidth = True
            Tabla.SetWidths({33, 33, 34})

            Dim Celda As PdfPCell = New PdfPCell


            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
            Celda.Colspan = 1
            Celda.BorderWidth = 0
            Celda.HorizontalAlignment = Element.ALIGN_LEFT
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(d.descripcionextra, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("COTIZACION", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)



            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(d.descripcion, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(cotizacion, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(d.direccion, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(Date.Now, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(d.nit, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(usuario, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            doc.Add(Tabla)
            doc.Add(PARRAFO_ESPACIO)


            Dim dp As datos = obtenerDatosCliente(idcliente)

            Dim TablaCliente As PdfPTable = New PdfPTable(2)

            TablaCliente.TotalWidth = 550.0F
            TablaCliente.LockedWidth = True
            TablaCliente.SetWidths({20, 80})

            Celda = New PdfPCell(New Paragraph("NIT: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(dp.nit, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("NOMBRE: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(dp.descripcion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("DIRECCION: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(dp.direccion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)



            'AGREGAMOS LA TABLA DE DATOS DEL PROVEEDOR
            doc.Add(TablaCliente)
            doc.Add(PARRAFO_ESPACIO)



            Dim TablaDatos As PdfPTable = New PdfPTable(5)

            TablaDatos.TotalWidth = 550.0F
            TablaDatos.LockedWidth = True
            TablaDatos.SetWidths({15, 40, 15, 15, 15})

            Celda = New PdfPCell(New Paragraph("CANTIDAD", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("NOMBRE", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("PRECIO UNITARIO", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("SUB-TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("UPC PROVEEDOR", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Dim total As Double = 0


            Dim SQL As String = "  SELECT c.Cantidad_Articulo, a.cod_Art,a.Des_Art, c.Precio_Unit_Articulo,c.Sub_Total, ISNULL(a.cod_pro1,' -- ') ups FROM [ERPDEVLYNGT].[dbo].[DET_COTIZACION]  c  " &
                "INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo] a on a.id_art = c.Id_Art WHERE id_enc =  " & cotizacion

            Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
            Dim Elemento As New datos
            For i = 0 To TablaEncabezado.Rows.Count - 1
                For ii = 0 To 1

                    Celda = New PdfPCell(New Paragraph(Format(TablaEncabezado.Rows(i).Item("Cantidad_Articulo"), "##,###"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(TablaEncabezado.Rows(i).Item("cod_Art") & " - " & TablaEncabezado.Rows(i).Item("Des_Art"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(Format(TablaEncabezado.Rows(i).Item("Precio_Unit_Articulo"), "##,##0.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(Format(TablaEncabezado.Rows(i).Item("Sub_Total"), "##,##0.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(TablaEncabezado.Rows(i).Item("ups").ToString, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 1
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    total += TablaEncabezado.Rows(i).Item("Sub_Total")

                    ii = ii + 1
                Next
            Next

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(Format(total, "##,##0.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)


            doc.Add(TablaDatos)

            doc.Add(PARRAFO_ESPACIO)

            Dim PARRAFO1 As Paragraph = New Paragraph("OBSERVACIONES: ", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD))
            PARRAFO1.Alignment = Element.ALIGN_LEFT
            PARRAFO1.SpacingAfter = 10.0F
            PARRAFO1.IndentationLeft = 25.0F
            PARRAFO1.IndentationRight = 25.0F
            doc.Add(PARRAFO1)

            Dim PARRAFO2 As Paragraph = New Paragraph(observacion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL))
            PARRAFO2.Alignment = Element.ALIGN_JUSTIFIED
            PARRAFO2.SpacingAfter = 10.0F
            PARRAFO2.IndentationLeft = 25.0F
            PARRAFO2.IndentationRight = 25.0F
            doc.Add(PARRAFO2)



            Dim PARRAFOFIRMA As Paragraph = New Paragraph("__________________________________________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL))
            PARRAFOFIRMA.Alignment = Element.ALIGN_CENTER
            PARRAFOFIRMA.SpacingBefore = 25.0F
            doc.Add(PARRAFOFIRMA)

            PARRAFOFIRMA = New Paragraph("Firma de Autorización", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL))
            PARRAFOFIRMA.Alignment = Element.ALIGN_CENTER
            doc.Add(PARRAFOFIRMA)


            doc.Close()


        Catch ex As Exception
            result = ex.Message
        End Try

        Return result
    End Function

    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer) As Integer
        Dim SQL As String = "Select Existencia_Deta_Art as cantidad from dbo.Existencias where Id_Bod = 1 AND Id_Art = " & idart

        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("cantidad")
            Next
        Next

        Return result

    End Function


    <WebMethod()>
    Public Function ObtenerCostoActual(ByVal idart As Integer) As Double
        Dim SQL As String = "SELECT e.Id_Art,  a.costo_art as costo FROM [ERPDEVLYNGT].[dbo].[Existencias] e INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo]  a ON  a.id_art = e.Id_Art where a.id_art  = " & idart

        Dim result As Double = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("costo")
            Next
        Next

        Return result

    End Function


    <WebMethod()>
    Public Function ObtenerSucursal(ByVal idusuario As String) As Integer
        Dim SQL As String = "SELECT id_sucursal  FROM [ERPDEVLYNGT].[dbo].[USUARIO] where USUARIO = '" & idusuario & "' "

        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("id_sucursal")
            Next
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function obtenerDatosEmpresa(ByVal idsuc As Integer) As datos
        Dim SQL As String = "SELECT  top 1 [id_empresa],[nombre],[nombre_comercial],[direccion],[nit]  FROM [ERPDEVLYNGT].[dbo].[ENCA_CIA] " &
            " where id_empresa = (SELECT  s.id_empresa  FROM [ERPDEVLYNGT].[dbo].[SUCURSALES]  s where s.id_suc = " & idsuc & ")"

        Dim result As datos = New datos()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        Dim Elemento As New datos
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                Elemento.id = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("nombre").ToString
                Elemento.descripcionextra = TablaEncabezado.Rows(i).Item("nombre_comercial").ToString
                Elemento.nit = TablaEncabezado.Rows(i).Item("nit").ToString
                Elemento.direccion = TablaEncabezado.Rows(i).Item("direccion").ToString
                ii = ii + 1
            Next
        Next

        Return Elemento

    End Function

    <WebMethod()>
    Public Function obtenerDatosCliente(ByVal id As Integer) As datos
        Dim SQL As String = "SELECT nit_clt,Id_Clt, Nom_clt, Dire_Clt FROM dbo.CLiente where Id_Clt = " & id

        Dim result As datos = New datos()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        Dim Elemento As New datos
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Clt")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_clt").ToString
                Elemento.direccion = TablaEncabezado.Rows(i).Item("Dire_Clt").ToString
                Elemento.nit = TablaEncabezado.Rows(i).Item("nit_clt").ToString
                ii = ii + 1
            Next
        Next

        Return Elemento

    End Function

    Public Class datos
        Public id As Integer
        Public descripcion As String
        Public descripcionextra As String
        Public direccion As String
        Public nit As String
    End Class


    Public Class productos
        Public id As Integer
        Public cantidad As Integer
        Public bo As String
        Public bodega As Integer
        Public precio As Double

    End Class


    Public Class pagos
        Public tipo As Integer
        Public valor As Double
        Public informacion As String
        Public tipoPagoText As String
        Public cambio As Double
    End Class
End Class