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
Public Class wsfacturacion
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Facturar(ByVal usuario As String, ByVal total As Double, ByVal descuento As Double, ByVal idcliente As Integer,
                             ByVal diascredito As Integer, ByVal listproductos As List(Of productos), ByVal listpagos As List(Of pagos),
                             ByVal efectivo As Double, ByVal cheques As Double, ByVal tarjeta As Double, ByVal valorExcencion As Double,
                             ByVal valorCertificado As Double, ByVal valorCredito As Double) As String
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

            Dim serie As String = ObtenerSerie(usuario)
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
            Dim str1 As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[ENC_FACTURA]([USUARIO],[id_empresa],[Serie_Fact],[Fecha],[firma],[Cae],[Total_Factura],[Iva_Factura],[Total_sin_iva],[Total_Descuento],[Id_Clt],[dias_cred],[id_suc],[efectivo],[cheques],[tarjeta],[valorExcencion],[valorCertificado],[valorCredito]) " &
                "VALUES('" & usuario & "', (" & empresa & "),'" & serie & "',GETDATE(),'" & firma & "','" & cae & "'," & total & "," & Math.Round(iva, 2) & "," & Math.Round(totalsiniva, 2) & "," & descuento & "," & idcliente & "," & diascredito & ", (" & sucursal & ")," & efectivo & "," & cheques & "," & tarjeta & "," & valorExcencion & "," & valorCertificado & "," & valorCredito & " )"

            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()


            'INSERTAMOS RECIVO
            Dim strRecivo As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[ENC_RECIBO] ([fecha],[Id_Clt],[Usuario],[empresa],[sucursal],[estado]) " &
                " VALUES(GETDATE()," & idcliente & ",'" & usuario & "',(" & empresa & "),(" & sucursal & "),1);"

            comando.CommandText = strRecivo
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE RECIVO
            comando.CommandText = "SELECT @@IDENTITY"
            Dim idRecivo As Integer = comando.ExecuteScalar()


            'INSERTAMOS EL RECIVO EN LA FACTURA
            Dim strFac_RECIVO As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[DET_RECIBO_FACT]([idRecibo],[id_enc],[abonado]) " &
                "VALUES(" & idRecivo & "," & id & "," & (total - valorCredito) & ");"

            comando.CommandText = strFac_RECIVO
            comando.ExecuteNonQuery()

            'INSERCION DEL DETALLE DE LA FACTURA
            For Each item As productos In listproductos

                Dim totalsinivaDesc As Double = (item.cantidad * item.precio) / 1.12
                Dim ivadesc As Double = totalsinivaDesc * 0.12

                Dim str2 As String = "INSERT INTO [ERPDEVLYNGT].[dbo].[DET_FACTURA] " &
                "([id_enc],[Cantidad_Articulo],[Precio_Unit_Articulo],[Sub_Total],[Descuento],[Iva],[Total_Sin_Iva],[Total],[Id_Art],[costoPromedio],[Id_Bod])" &
                "VALUES(" & id & "," & item.cantidad & "," & item.precio & "," & (item.cantidad * item.precio) & ",0.00," & Math.Round(ivadesc, 2) & "," & Math.Round(totalsinivaDesc, 2) & "," & Math.Round((item.cantidad * item.precio), 2) & "," & item.id & ", ROUND((select CONVERT(varchar,costo_art) from [ERPDEVLYNGT].[dbo].[Articulo] where id_art = " & item.id & "),2)," & item.bodega & ");"

                Dim existencia_actual As Integer = ObtenerCantidadProducto(item.id, item.bodega)

                str2 = str2 + " UPDATE [dbo].[Existencias] SET [Existencia_Deta_Art] = " & existencia_actual - item.cantidad & " WHERE Id_Bod = " & item.bodega & " and Id_Art = " & item.id

                comando.CommandText = str2
                comando.ExecuteNonQuery()
            Next


            'INSERCION DEL DETALLE DEL RECIVO (METODOS DE PAGO)

            For Each item As pagos In listpagos
                Dim StrPago As String = "INSERT INTO [dbo].[DET_RECIBO]([idRecibo],[tipoPago],[documento],[valor]) " &
                    "VALUES(" & idRecivo & ",'" & item.tipo & "','" & item.informacion & "'," & item.valor - item.cambio & ")"

                comando.CommandText = StrPago
                comando.ExecuteNonQuery()


                If item.tipo = 8 Then
                    Dim strCupon As String = ""
                    If item.cambio = 0 Then
                        strCupon = "UPDATE [ERPDEVLYNGT].[dbo].[DETA_CUPON] set saldo  = 0   WHERE estado  = 1 and id_cupon_detalle = " & item.extra
                    Else
                        strCupon = "UPDATE [ERPDEVLYNGT].[dbo].[DETA_CUPON] set saldo  = " & item.valor - (item.valor - item.cambio) & "   WHERE estado  = 1 and id_cupon_detalle = " & item.extra
                    End If

                    comando.CommandText = strCupon
                    comando.ExecuteNonQuery()

                End If



            Next


            transaccion.Commit()


            result = "SUCCESS| DATOS FACTURADOS EXITOSAMENTE|" & CrearPDF(usuario, listproductos, listpagos, descuento, serie, firma, cae, idcliente)


        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            result = "Error|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer, ByVal idbodega As Integer) As Integer
        Dim SQL As String = "Select Existencia_Deta_Art As cantidad from ERPDEVLYNGT.dbo.Existencias where Id_Art = " & idart & " And id_bod = " & idbodega

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
    Public Function ObtenerSerie(ByVal usuario As String) As String
        Dim SQL As String = "select top 1 Series from [ERPDEVLYNGT].[dbo].[SUCURSALES] S INNER JOIN [ERPDEVLYNGT].[dbo].[Correlativos] C ON c.id_correlativo = s.id_correlativo where id_suc = (select u.id_sucursal from  [ERPDEVLYNGT].[dbo].[USUARIO] u where USUARIO = '" & usuario & "')"

        Dim result As String = ""
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("Series")
            Next
        Next

        Return result

    End Function


    <WebMethod()>
    Public Function CrearPDF(ByVal usuario As String, ByVal listproductos As List(Of productos), ByVal listpagos As List(Of pagos), ByVal descuento As Double, ByVal Serie As String, ByVal firma As String, ByVal cae As String, ByVal idcliente As Integer) As String
        Dim result As String = ""
        Try

            Dim doc As Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 5, 5, 5, 5)
            Dim datafecha As Date = Now
            Dim nombredoc As String = "pdf\factura" & Serie & firma & ".pdf"
            Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(Server.MapPath("~\vista\" & nombredoc), FileMode.Create))
            result = nombredoc
            doc.AddTitle("FACTURA ")
            doc.AddAuthor("")
            doc.AddCreationDate()

            doc.Open()

            Dim PARRAFO_ESPACIO As Paragraph = New Paragraph(" ", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD))
            PARRAFO_ESPACIO.Alignment = Element.ALIGN_CENTER

            Dim d As datos = obtenerDatosEmpresa(usuario)

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

            Celda = New PdfPCell(New Paragraph("VENTA DE MERCADERIA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
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

            Celda = New PdfPCell(New Paragraph("", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
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



            Dim dp As datos = ObtenerDatosCliente(idcliente)

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




            Dim TablaDatos As PdfPTable = New PdfPTable(6)

            TablaDatos.TotalWidth = 550.0F
            TablaDatos.LockedWidth = True
            TablaDatos.SetWidths({15, 35, 14, 20, 8, 8})

            Celda = New PdfPCell(New Paragraph("CODIGO", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
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

            Celda = New PdfPCell(New Paragraph("BODEGA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("CANTIDAD", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("PRECIO", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("SUB-TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)


            Dim total As Double = 0

            For Each item As productos In listproductos
                Celda = New PdfPCell(New Paragraph(item.codigo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.HorizontalAlignment = Element.ALIGN_LEFT
                TablaDatos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(item.descripcion, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_LEFT
                TablaDatos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(item.cantidad, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_CENTER
                TablaDatos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(item.bo, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_LEFT
                TablaDatos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(Format(item.precio, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 1
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                TablaDatos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(Format(item.precio * item.cantidad, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 1
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                TablaDatos.AddCell(Celda)


                total += item.precio * item.cantidad

            Next


            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("SUB-TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(Format(total, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_RIGHT
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("DESCUENTO", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(Format(descuento, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_RIGHT
            TablaDatos.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(Format(total - descuento, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_RIGHT
            TablaDatos.AddCell(Celda)

            doc.Add(TablaDatos)

            doc.Add(PARRAFO_ESPACIO)
            doc.Add(PARRAFO_ESPACIO)


            Dim PARRAFO_INFO As Paragraph = New Paragraph(" INFORMACION DE PAGO ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL))
            PARRAFO_INFO.Alignment = Element.ALIGN_CENTER
            doc.Add(PARRAFO_INFO)
            doc.Add(PARRAFO_ESPACIO)
            doc.Add(PARRAFO_ESPACIO)
            Dim TablaPagos As PdfPTable = New PdfPTable(5)

            TablaPagos.TotalWidth = 550.0F
            TablaPagos.LockedWidth = True
            TablaPagos.SetWidths({25, 25, 17, 16, 17})

            Celda = New PdfPCell(New Paragraph("FORMA DE PAGO", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("DESCRIPCION", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("VALOR", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("CAMBIO", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph("SUB-TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)

            Dim total_pago As Double = 0


            For Each item As pagos In listpagos
                Celda = New PdfPCell(New Paragraph(item.tipoPagoText, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                TablaPagos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(item.informacion, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_LEFT
                TablaPagos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(Format(item.valor, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                TablaPagos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(Format(item.cambio, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 0
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_LEFT
                TablaPagos.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(Format(item.valor - item.cambio, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 1
                Celda.BorderWidthTop = 0
                Celda.BorderWidthRight = 1
                Celda.Colspan = 0
                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                TablaPagos.AddCell(Celda)

                total_pago += item.valor - item.cambio

            Next





            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("--", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(Format(total_pago, "##,###.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_RIGHT
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaPagos.AddCell(Celda)


            doc.Add(TablaPagos)

            Dim PARRAFO_SERIE As Paragraph = New Paragraph(Serie & "-" & firma, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD))
            PARRAFO_SERIE.Alignment = Element.ALIGN_CENTER
            doc.Add(PARRAFO_SERIE)
            PARRAFO_SERIE = New Paragraph(cae, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD))
            PARRAFO_SERIE.Alignment = Element.ALIGN_CENTER
            doc.Add(PARRAFO_SERIE)



            doc.Close()


        Catch ex As Exception
            result = ex.Message
        End Try

        Return result
    End Function


    <WebMethod()>
    Public Function obtenerDatosEmpresa(ByVal usuario As String) As datos
        Dim SQL As String = "SELECT  top 1 [id_empresa],[nombre],[nombre_comercial],[direccion],[nit]  FROM [ERPDEVLYNGT].[dbo].[ENCA_CIA] " &
            " where id_empresa = (select u.id_empresa from [dbo].[USUARIO] u where u.USUARIO = '" & usuario & "')"

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
        Dim SQL As String = "Select ISNULL(MAX(CAST(SUBSTRING(firma, 10, 25) As numeric)), 180000000000) + 1 As Siguiente FROM [ERPDEVLYNGT].[dbo].[ENC_FACTURA] WHERE Serie_Fact = '" & serie & "';"

        Dim result As String = ""
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            result = TablaEncabezado.Rows(i).Item("Siguiente").ToString
        Next

        Return result

    End Function


    <WebMethod()>
    Public Function ObtenerValorDelCupon(ByVal serie As String, ByVal idcliente As Integer) As String
        Dim SQL As String = "SELECT d.saldo, d. id_cupon_detalle FROM [ERPDEVLYNGT].[dbo].[DETA_CUPON] d  INNER JOIN [ERPDEVLYNGT].[dbo].[ENCA_CUPON] EC ON EC.id_cupon = d.id_cupon  WHERE d.estado  = 1 and d.saldo > 0 and d.serie = '" & serie & "'  and EC.idcliente  = " & idcliente

        Dim result As String = ""
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        result = "ERROR|ERROR PUEDA QUE NO EXISTA EL CUPON, NO POSEA SALDO O EL CLIENTE SEA INCORRECTO"
        For i = 0 To TablaEncabezado.Rows.Count - 1
            result = "SUCCESS|" & TablaEncabezado.Rows(i).Item("saldo").ToString & "|" & TablaEncabezado.Rows(i).Item("id_cupon_detalle").ToString
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function obtenerDatosCliente(ByVal id As Integer) As datos
        Dim SQL As String = "SELECT Id_Clt,nit_clt,Nom_clt,Dire_Clt FROM [ERPDEVLYNGT].[dbo].[CLiente] WHERE Id_Clt = " & id

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


    Public Class productos
        Public id As Integer
        Public cantidad As Integer
        Public bo As String
        Public bodega As Integer
        Public precio As Double
        Public descripcion As String
        Public codigo As String
    End Class


    Public Class pagos
        Public tipo As Integer
        Public valor As Double
        Public informacion As String
        Public tipoPagoText As String
        Public cambio As Double
        Public extra As Integer
        Public pago As Double
    End Class

    Public Class datos
        Public id As Integer
        Public descripcion As String
        Public descripcionextra As String
        Public direccion As String
        Public nit As String
    End Class


End Class