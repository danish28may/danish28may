using iTextSharp.text.pdf;
using org.pdfbox.pdmodel;
using org.pdfbox.pdmodel.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaypeePDFSettings
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() > 0 && args[0].ToLower().EndsWith(".pdf") && (args.Count() == 1 || args.Count() == 2))
            {
                Console.WriteLine("File path: " + args[0]);

                if (args.Length == 1)
                 ApplyPDFSettings(args[0], 1);
                else
                 ApplyPDFSettings(args[0], Convert.ToInt16(args[1]));


                Console.WriteLine("PDF settings applied Successfully!");
            }
            //////Added on 12-10-2021 for issue building pdf settings apply by vikas
            else if (args.Count() > 0 && args[0].ToLower().EndsWith(".pdf") && args.Count() == 3)
            {
                Console.WriteLine("File path: " + args[0] + " and "+ args[2]);

                ApplyPDFSettingsIssuepdf(args[0], args[2]);

                Console.WriteLine("Issue PDF settings applied Successfully!");
            }
            else
            {
                Console.WriteLine("Wrong arugument given to application.\n Argument should be pdf file!");

            }
        }

        public static void ApplyPDFSettings(string pdfpath, int PassYN)
        {
            try
            {
                ////Itextsharp for change pdf meta dat crator name,apply Initial view settings Navigation tab = Bookmarks Panel and Page ,Page Layout = Single Page ,Magnification = Fit Width
                var originalfile = pdfpath.Replace(".pdf", "_Org.pdf").Replace(".PDF", "_Org.PDF");
                var dirc = new FileInfo(originalfile).Directory.FullName;
                /////Take backup of original PDF file
                if (Directory.Exists(dirc))
                {
                    var createnewdir = dirc + "\\" + "OriginalWebPDF";
                    if (!Directory.Exists(createnewdir))
                    {
                        Directory.CreateDirectory(createnewdir);
                    }
                    if (Directory.Exists(createnewdir))
                        File.Copy(pdfpath, createnewdir + "\\" + new FileInfo(originalfile).Name, true);

                }
                /////crate copy of original file for read and update propertis and create new pdf file
                File.Copy(pdfpath, originalfile, true);
                if (File.Exists(pdfpath))
                {
                    File.Delete(pdfpath);
                }

                iTextSharp.text.pdf.PdfReader readers = new iTextSharp.text.pdf.PdfReader(originalfile);



                PdfStamper stampers = new PdfStamper(readers, new FileStream(pdfpath, FileMode.Create));
                
                Dictionary<String, String> infos = readers.Info;
                //infos["Creator"] = System.Configuration.ConfigurationManager.AppSettings["pdfcreator"];
                // stampers.MoreInfo = infos;
                PdfDestination pdfDest = new PdfDestination(PdfDestination.FIT, stampers.Writer.GetVerticalPosition(false));
                PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, stampers.Writer);
                stampers.Writer.SetOpenAction(action);
                stampers.Writer.ViewerPreferences = PdfWriter.PageModeUseOutlines | PdfWriter.PageLayoutSinglePage;
                stampers.Close();
                readers.Close();

                /////PDFBox library used for change PDF Producer               
                ChangePDFProducer(pdfpath);

                if (PassYN == 0)
                    ProtectPDF(pdfpath);
                
                //////After all process done as already tacken backup for original pdf file in OriginalWebPDF folder so delete this file which we rquired for only read by iTextsharp code
                if (File.Exists(originalfile))
                {
                    File.Delete(originalfile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:", ex.Message + "\n" + ex.StackTrace);
            }
        }

        /*  public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType)
          {
              if (pdf.IsOpen())
                  throw new DocumentException(MessageLocalization.GetComposedMessage("encryption.can.only.be.added.before.opening.the.document"));
              crypto = new PdfEncryption();
              crypto.SetCryptoMode(encryptionType, 0);
              crypto.SetupAllKeys(userPassword, ownerPassword, permissions);
          }*/

        public static void ProtectPDF(string pdfpath)
        {
            string srcFile = pdfpath;
            string tarFile = pdfpath.Replace(".pdf", "PS.pdf");

            try
            {
            

                  PdfReader reader = new PdfReader(srcFile);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        // add your content
                    }
                    using (FileStream fs = new FileStream(tarFile, FileMode.Create, FileAccess.ReadWrite))
                    {
                        PdfEncryptor.Encrypt(
                          new PdfReader(ms.ToArray()),
                          fs,
                          null,
                          System.Text.Encoding.UTF8.GetBytes("ditech@123"),
                          PdfWriter.ALLOW_PRINTING
                              | PdfWriter.ALLOW_COPY
                              | PdfWriter.ALLOW_MODIFY_ANNOTATIONS,
                          true
                        );
                    }
                }
                reader.Close();
            }
            catch (Exception Ex)
            { }
            finally
            {
                try
                {
                 

                    if (File.Exists(tarFile))
                    {
                        if (File.Exists(srcFile))
                        {
                            
                            File.Delete(srcFile);
                            File.Copy(tarFile, pdfpath);
                            File.Delete(tarFile);
                        }                       
                    }
                }
                catch (Exception Ex)
                {
                                  }             
            }
            }
    

        public static void ProtectPDF_1(string pdfpath)
        {
            try
            {
                ////Itextsharp for change pdf meta dat crator name,apply Initial view settings Navigation tab = Bookmarks Panel and Page ,Page Layout = Single Page ,Magnification = Fit Width
                var originalfile = pdfpath.Replace(".pdf", "_Org.pdf").Replace(".PDF", "_Org.PDF");
                var dirc = new FileInfo(originalfile).Directory.FullName;
                /////Take backup of original PDF file
            

                iTextSharp.text.pdf.PdfReader readers = new iTextSharp.text.pdf.PdfReader(pdfpath);

              


               // PdfStamper stampers = new PdfStamper(readers, new FileStream(pdfpath, FileMode.Create));

             //    PdfEncryptor.Encrypt(readers, pdfpath.Replace(".pdf","11.pdf"), true, "ditech@123", "ditech@123", PdfWriter.ALLOW_SCREENREADERS);
                              
                readers.Close();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:", ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ApplyPDFSettingsIssuepdf(string pdfpath, string subject)
        {
            try
            {
                ////Itextsharp for change pdf meta dat crator name,apply Initial view settings Navigation tab = Bookmarks Panel and Page ,Page Layout = Single Page ,Magnification = Fit Width
                var originalfile = pdfpath.Replace(".pdf", "_Org.pdf").Replace(".PDF", "_Org.PDF");
                var dirc = new FileInfo(originalfile).Directory.FullName;
                /////Take backup of original PDF file
                if (Directory.Exists(dirc))
                {
                    var createnewdir = dirc + "\\" + "OriginalWebPDF";
                    if (!Directory.Exists(createnewdir))
                    {
                        Directory.CreateDirectory(createnewdir);
                    }
                    if (Directory.Exists(createnewdir))
                        File.Copy(pdfpath, createnewdir + "\\" + new FileInfo(originalfile).Name, true);

                }
                /////crate copy of original file for read and update propertis and create new pdf file
                File.Copy(pdfpath, originalfile, true);
                if (File.Exists(pdfpath))
                {
                    File.Delete(pdfpath);
                }

                iTextSharp.text.pdf.PdfReader readers = new iTextSharp.text.pdf.PdfReader(originalfile);
                PdfStamper stampers = new PdfStamper(readers, new FileStream(pdfpath, FileMode.Create));
                Dictionary<String, String> infos = readers.Info;
                //infos["Creator"] = System.Configuration.ConfigurationManager.AppSettings["pdfcreator"];
                // stampers.MoreInfo = infos;
                
                PdfDestination pdfDest = new PdfDestination(PdfDestination.FIT, stampers.Writer.GetVerticalPosition(false));
                PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, stampers.Writer);
                stampers.Writer.SetOpenAction(action);
                stampers.Writer.ViewerPreferences = PdfWriter.PageModeUseOutlines | PdfWriter.PageLayoutSinglePage;
                stampers.Close();
                readers.Close();

                /////PDFBox library used for change PDF Producer               
                ChangeIssuePDFProducer(pdfpath, subject);

                //////After all process done as already tacken backup for original pdf file in OriginalWebPDF folder so delete this file which we rquired for only read by iTextsharp code
                if (File.Exists(originalfile))
                {
                    File.Delete(originalfile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:", ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ChangePDFProducer(string pdfpath)
        {
            PDDocument doc = PDDocument.load(pdfpath);
            org.pdfbox.pdmodel.PDDocumentInformation pdd = doc.getDocumentInformation();
            var text = pdd.getProducer();
            doc.setDocumentInformation(pdd);
            PDDocumentCatalog cat = doc.getDocumentCatalog();
            PDMetadata metadata = cat.getMetadata();
            byte[] bytes = metadata.getByteArray();
            String str = Encoding.UTF8.GetString(bytes);
            //str = str.Replace("Adobe PDF Library 15.0", "jaypee");

            ///////xml return this string "Adobe PDF Library 15.0; modified using iTextSharp™ 5.5.13.2 ©2000 - 2020 iText Group NV(AGPL - version)" and text object return Adobe PDF Library 15.0; modified using iTextSharp\u0092 5.5.13.2 ©2000-2020 iText Group NV (AGPL-version)
            //////here required Producer andcreator text is added in xml by replacing old values
            str = str.Replace("iTextSharp™", "").Replace(text.Replace("iTextSharp\u0092", ""), System.Configuration.ConfigurationManager.AppSettings["pdfproducer"]).Replace(System.Configuration.ConfigurationManager.AppSettings["oldpdfcreator"], System.Configuration.ConfigurationManager.AppSettings["pdfcreator"]);
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);
            java.io.ByteArrayInputStream JavaDoc = new java.io.ByteArrayInputStream(byteArray);
            PDMetadata metadataStream = new PDMetadata(doc, JavaDoc, false);
            cat.setMetadata(metadataStream);
            ////first reset producer data then set here
            pdd.setProducer(System.Configuration.ConfigurationManager.AppSettings["pdfproducer"]);
            pdd.setCreator(System.Configuration.ConfigurationManager.AppSettings["pdfcreator"]);

            metadata = cat.getMetadata();
            bytes = metadata.getByteArray();
            str = Encoding.UTF8.GetString(bytes);
            str = str.Replace("iTextSharp™", "").Replace(text.Replace("iTextSharp\u0092", ""), " ");
            byteArray = Encoding.UTF8.GetBytes(str);
            doc.save(pdfpath);
            doc.close();
        }

        public static void ChangeIssuePDFProducer(string pdfpath, string subject)
        {
            PDDocument doc = PDDocument.load(pdfpath);
            org.pdfbox.pdmodel.PDDocumentInformation pdd = doc.getDocumentInformation();
            var text = pdd.getProducer();
            doc.setDocumentInformation(pdd);
            PDDocumentCatalog cat = doc.getDocumentCatalog();
            PDMetadata metadata = cat.getMetadata();
            byte[] bytes = metadata.getByteArray();
            String str = Encoding.UTF8.GetString(bytes);
            //str = str.Replace("Adobe PDF Library 15.0", "jaypee");

            ///////xml return this string "Adobe PDF Library 15.0; modified using iTextSharp™ 5.5.13.2 ©2000 - 2020 iText Group NV(AGPL - version)" and text object return Adobe PDF Library 15.0; modified using iTextSharp\u0092 5.5.13.2 ©2000-2020 iText Group NV (AGPL-version)
            //////here required Producer andcreator text is added in xml by replacing old values
            str = str.Replace("iTextSharp™", "").Replace(text.Replace("iTextSharp\u0092", ""), System.Configuration.ConfigurationManager.AppSettings["pdfproducer"]).Replace(System.Configuration.ConfigurationManager.AppSettings["oldpdfcreator"], System.Configuration.ConfigurationManager.AppSettings["pdfcreator"]);
            var xdoc = System.Xml.Linq.XDocument.Parse(str);
            if (xdoc.Descendants().Where(x => x.Name.LocalName == "Description").Count() > 0)
            {
                var descriptionnode = xdoc.Descendants().Where(x => x.Name.LocalName == "Description").FirstOrDefault();
                if (descriptionnode.Attributes().Where(x => x.Name.LocalName == "Keywords").Count() > 0)
                {
                    var attribute = descriptionnode.Attributes().Where(x => x.Name.LocalName == "Keywords").FirstOrDefault();
                    attribute.Value = "";
                }
            }
            if (xdoc.Descendants().Where(x => x.Name.LocalName == "description").Count() > 0)
            {
                var descriptionnode = xdoc.Descendants().Where(x => x.Name.LocalName == "description").FirstOrDefault();
                if (descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).Count() > 0)
                {
                    var attribute = descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).FirstOrDefault();
                    ((System.Xml.Linq.XText)attribute).Value = subject.Replace("ΓÇô", "–");
                }
            }
            if (xdoc.Descendants().Where(x => x.Name.LocalName == "title").Count() > 0)
            {
                var descriptionnode = xdoc.Descendants().Where(x => x.Name.LocalName == "title").FirstOrDefault();
                if (descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).Count() > 0)
                {
                    var attribute = descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).FirstOrDefault();
                    ((System.Xml.Linq.XText)attribute).Value = "";
                }
            }
            if (xdoc.Descendants().Where(x => x.Name.LocalName == "creator").Count() > 0)
            {
                var descriptionnode = xdoc.Descendants().Where(x => x.Name.LocalName == "creator").FirstOrDefault();
                if (descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).Count() > 0)
                {
                    var attribute = descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).FirstOrDefault();
                    ((System.Xml.Linq.XText)attribute).Value = "";
                }
            }
            if (xdoc.Descendants().Where(x => x.Name.LocalName == "subject").Count() > 0)
            {
                var descriptionnode = xdoc.Descendants().Where(x => x.Name.LocalName == "subject").FirstOrDefault();
                if (descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).Count() > 0)
                {
                    var attributes = descriptionnode.DescendantNodes().Where(x => x.NodeType == System.Xml.XmlNodeType.Text).ToList();
                    attributes.ToList().ForEach(x =>
                    {
                        ((System.Xml.Linq.XText)x).Value = "";
                    });
                    
                }
            }
            str = xdoc.ToString();

            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);
            java.io.ByteArrayInputStream JavaDoc = new java.io.ByteArrayInputStream(byteArray);
            PDMetadata metadataStream = new PDMetadata(doc, JavaDoc, false);
            cat.setMetadata(metadataStream);
            ////first reset producer data then set here
            pdd.setProducer(System.Configuration.ConfigurationManager.AppSettings["pdfproducer"]);
            pdd.setCreator(System.Configuration.ConfigurationManager.AppSettings["pdfcreator"]);
            pdd.setTitle("");
            pdd.setAuthor("");
            pdd.setKeywords("");

            metadata = cat.getMetadata();
            bytes = metadata.getByteArray();
            str = Encoding.UTF8.GetString(bytes);
            str = str.Replace("iTextSharp™", "").Replace(text.Replace("iTextSharp\u0092", ""), " ");

            byteArray = Encoding.UTF8.GetBytes(str);
          
            doc.save(pdfpath);
            doc.close();
        }

        public static void ChangeIssuePDfmeta(string pdfpath, string subject)
        {
            PDDocument doc = PDDocument.load(pdfpath);
            org.pdfbox.pdmodel.PDDocumentInformation pdd = doc.getDocumentInformation();
            var text = pdd.getProducer();
            var textauthor = pdd.getAuthor();
            var texttitle = pdd.getTitle();
            var textsub = pdd.getSubject();
            var textkeywords = pdd.getKeywords();
            pdd.setAuthor("");
            pdd.setTitle("");
            pdd.setKeywords("");
            pdd.setSubject(subject);
           // doc.setDocumentInformation(pdd);                    
            doc.save(pdfpath);
            doc.close();
        }

    }
}
