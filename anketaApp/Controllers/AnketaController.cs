using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using anketaApp.Models;
using System.IO;
//using Microsoft.Office.Interop.Excel;
using Ionic.Zip;
using OfficeOpenXml;

namespace anketaApp.Controllers
{
    public class AnketaController : Controller
    {

        private anketaEntities db = new anketaEntities();

        //Просмотр анкеты по идентификатору
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            form form = db.form.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        //Добавление нового ответа на заданную анкету
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddAnswers(int form_id)
        {
            List<question> quest = db.question.Where(p => p.id_form == form_id).ToList(); //Список вопросов данной формы
            List<question_else> list_else = db.question_else.Where(p => p.question.id_form == form_id).ToList(); //Список вариантов "Другое" у вопросов данной формы

            List<answer_text> answer_text = new List<answer_text>(); //Список для текстовых ответов на вопрос
            List<answer_table> answer_table = new List<answer_table>(); //Список для ответов на табличные вопросы
            List<answer_variant> asnwer_variant = new List<answer_variant>(); //Список ответов на вопросы с типами от 2 до 4
            List<answer_else> answer_else = new List<answer_else>(); //Список для ответов, содержащихся в поле "другое"
            int id1 = (db.answer_text.Count()>0)?(db.answer_text.Max(p=>p.id)+1):1, id2 = (db.answer_table.Count() > 0) ? (db.answer_table.Max(p => p.id) + 1) : 1, id3 = (db.answer_variant.Count() > 0) ? (db.answer_variant.Max(p => p.id) + 1) : 1, id4 = (db.answer_else.Count() > 0) ? (db.answer_else.Max(p => p.id) + 1) : 1;

            user user = new user();
            user.id = (db.user.Count() > 0) ? (db.user.Max(p => p.id) + 1) : 1;
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    user.ip= addresses[0];
                }
            }
            else
            {
                user.ip = context.Request.ServerVariables["REMOTE_ADDR"]; ;
            }
            user.id_subject = Convert.ToInt32(Request.Form["subject"]);
            user.id_school = Convert.ToInt32(Request.Form["school"]);
            db.user.Add(user);
            db.SaveChanges();

            foreach (var item in quest)
            {
                if (item.id_type_question == 1)
                {
                    string result = Request.Form["text_" + item.id];
                    if (result != null)
                    {
                        answer_text a = new answer_text();
                        a.id = id1++;
                        a.id_question = item.id;
                        a.name = result;
                        a.id_user = user.id;
                        answer_text.Add(a);
                    }
                }
                else if (item.id_type_question == 2)
                {
                    string result = Request.Form["radio_" + item.id];
                    if (result != null)
                    {
                        answer_variant a = new answer_variant();
                        a.id = id3++;
                        a.id_variant = Convert.ToInt32(result);
                        a.id_user = user.id;
                        asnwer_variant.Add(a);
                    }
                }
                else if (item.id_type_question == 3)
                {
                    string result = Request.Form["checkbox_" + item.id];
                    if (result != null)
                    {
                        string[] arr = result.Split(',');
                        foreach(var i in arr)
                        {
                            answer_variant a = new answer_variant();
                            a.id = id3++;
                            a.id_variant = Convert.ToInt32(i);
                            a.id_user = user.id;
                            asnwer_variant.Add(a);
                        } 
                    }
                }
                else if (item.id_type_question == 4)
                {
                    string result = Request.Form["select_" + item.id];
                    if (result != null)
                    {
                        answer_variant a = new answer_variant();
                        a.id = id3++;
                        a.id_variant = Convert.ToInt32(result);
                        a.id_user = user.id;
                        asnwer_variant.Add(a);
                    }
                }
                else if (item.id_type_question == 5)
                {
                    List<table_question> t_q = db.table_question.Where(p => p.id_question == item.id).ToList();
                    foreach(var q in t_q)
                    {
                        string result = Request.Form["table_" + item.id + "_" + q.id];
                        if (result != null)
                        {
                            answer_table a = new answer_table();
                            a.id = id2++;
                            a.id_table_variant= Convert.ToInt32(result);
                            a.id_table_question = q.id;
                            a.id_user = user.id;
                            answer_table.Add(a);
                        }
                    }
                }
                else if ( item.id_type_question == 6)
                {
                    List<table_question> t_q = db.table_question.Where(p => p.id_question == item.id).ToList();
                    foreach (var q in t_q)
                    {
                        string result = Request.Form["table_" + item.id + "_" + q.id];
                        if (result != null)
                        {
                            string[] arr = result.Split(',');
                            foreach(var i in arr)
                            {
                                answer_table a = new answer_table();
                                a.id = id2++;
                                a.id_table_variant = Convert.ToInt32(i);
                                a.id_table_question = q.id;
                                a.id_user = user.id;
                                answer_table.Add(a);
                            }
                        }
                    }
                }
                else if (item.id_type_question == 7)
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files["file_" + item.id];

                        if (file != null && file.ContentLength > 0)
                        {
                            string newName = String.Format("File_{0}_{1}{2}",DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(),Path.GetExtension(file.FileName));
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/files/"), newName);
                            file.SaveAs(path);
                            variant_file v = db.variant_file.First(p => p.question_id == item.id);
                            answer_file a = new answer_file();
                            a.id = (db.answer_file.Count() > 0) ? (db.answer_file.Max(p => p.id) + 1) : 1;
                            a.link = "~/Content/files/" + newName;
                            a.user_id = user.id;
                            a.variant_file_id = v.id;
                            db.answer_file.Add(a);
                            db.SaveChanges();
                        }
                    }
                }
                if ((item.id_type_question == 1 || item.id_type_question == 2 || item.id_type_question == 3) && list_else.Count(p=>p.id_question==item.id)>0)
                {
                    string result = Request.Form["else_" + item.id];
                    if (result != null && !result.Equals(""))
                    {
                        question_else q_e = list_else.First(p => p.id_question == item.id);
                        answer_else a_e = new answer_else();
                        a_e.id = id4++;
                        a_e.id_question_else = q_e.id;
                        a_e.name = result;
                        a_e.id_user = user.id;
                        answer_else.Add(a_e);
                    }
                }
            }
            db.answer_text.AddRange(answer_text);
            db.answer_variant.AddRange(asnwer_variant);
            db.answer_table.AddRange(answer_table);
            db.answer_else.AddRange(answer_else);
            db.SaveChanges();
            return Redirect("/Anketa/Send?form_id=" + form_id);
        }

        //Результаты (ответы) по выбранной анкете в файле .excel
        public ActionResult ExportExcel(int form_id)
        {
            int rows = 1;
            int cols = 5;
            List<answer_text> answer_text = db.answer_text.Where(p => p.question.id_form == form_id).ToList();
            List<answer_variant> answer_variant=db.answer_variant.Where(p => p.variant.question.id_form == form_id).ToList();
            List<answer_table> answer_table = db.answer_table.Where(p => p.table_question.question.id_form == form_id).ToList();
            List<answer_file> answer_file = db.answer_file.Where(p => p.variant_file.question.id_form == form_id).ToList();
            List<answer_else> answer_else = db.answer_else.Where(p => p.question_else.question.id_form == form_id).ToList();
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Результаты");
            List<question> list = db.question.Where(p => p.id_form == form_id).OrderBy(p=>p.number).ToList();
            List<user> users = db.user.Where(p=>p.answer_text.Count(s=>s.question.id_form==form_id)>0 || p.answer_variant.Count(s => s.variant.question.id_form == form_id) >0 || p.answer_table.Count(s=>s.table_question.question.id_form==form_id)>0 || p.answer_file.Count(s=>s.variant_file.question.id_form==form_id)>0).ToList();
            workSheet.Cells[1, 1].Value = "Область";
            workSheet.Cells[1, 2].Value = "Район";
            workSheet.Cells[1, 3].Value = "Школа";
            workSheet.Cells[1, 4].Value = "Предмет";
            rows = 3;
            for (int s = 0; s < users.Count(); ++s)
            {
                user u = users.ElementAt(s);
                workSheet.Cells[rows, 1].Value = u.schools.districts.areas.name;
                workSheet.Cells[rows, 2].Value = u.schools.districts.name;
                workSheet.Cells[rows, 3].Value = u.schools.name;
                workSheet.Cells[rows++, 4].Value = u.subjects.name;
            }
            for (int i = 0; i < list.Count(); ++i)
            {
                question q = list.ElementAt(i);
                workSheet.Cells[1, cols].Value = list.ElementAt(i).name;
                rows = 3;
                if (list.ElementAt(i).id_type_question == 1)
                {
                    for (int j = 0; j < users.Count(); ++j)
                    {
                        user u = users.ElementAt(j);
                        answer_text a1 = answer_text.FirstOrDefault(p => p.id_user == u.id && p.id_question == q.id);
                        workSheet.Cells[rows++, cols].Value = (a1 != null) ? a1.name : "";
                    }
                    cols++;
                }
                else if (list.ElementAt(i).id_type_question ==2 || list.ElementAt(i).id_type_question==4)
                {
                    for (int j = 0; j < users.Count(); ++j)
                    {
                        user u = users.ElementAt(j);
                        answer_variant a2 = answer_variant.FirstOrDefault(p => p.id_user == u.id && p.variant.id_question == q.id);
                        string ask = (a2 != null) ? a2.variant.name : "";
                        if (answer_else.FirstOrDefault(p => p.question_else.id_question == q.id && p.id_user==u.id) != null)
                        {
                            if (a2 != null)
                            {
                                ask += ", ";
                            }
                            ask += answer_else.FirstOrDefault(p => p.question_else.id_question == q.id && p.id_user == u.id).name;
                        }
                        workSheet.Cells[rows++, cols].Value = ask;
                    }
                    cols++;
                }
                else if (list.ElementAt(i).id_type_question == 3)
                {
                    for (int j = 0; j < users.Count(); ++j)
                    {
                        user u = users.ElementAt(j);
                        List<answer_variant> a2 = answer_variant.Where(p => p.id_user == u.id && p.variant.id_question == q.id).ToList();
                        string res= "";
                        for(int k = 0; k < a2.Count(); ++k)
                        {
                            res += a2.ElementAt(k).variant.name;
                            if (k != (a2.Count() - 1)){
                                res += ", ";
                            }
                        }
                        answer_else el = answer_else.FirstOrDefault(p => p.question_else.id_question == q.id && p.id_user == u.id);
                        if (el != null)
                        {
                            res += ", " + el.name;
                        }
                        workSheet.Cells[rows++, cols].Value = res;
                    }
                    cols++;
                }
                else if (list.ElementAt(i).id_type_question == 5)
                {
                    List<table_question> table_question = db.table_question.Where(p => p.id_question == q.id).ToList();
                    for(int k = 0; k < table_question.Count(); ++k)
                    {
                        table_question t_q = table_question.ElementAt(k);
                        workSheet.Cells[2, cols].Value = t_q.name;
                        rows = 3;
                        for (int j = 0; j < users.Count(); ++j)
                        {
                            user u = users.ElementAt(j);
                            answer_table a3 = answer_table.FirstOrDefault(p => p.id_user == u.id && p.id_table_question == t_q.id);
                            workSheet.Cells[rows++, cols].Value = (a3 != null) ? a3.table_variant.name : "";
                        }
                        cols++;
                    }
                }
                else if (list.ElementAt(i).id_type_question == 6)
                {
                    List<table_question> table_question = db.table_question.Where(p => p.id_question == q.id).ToList();
                    for (int k = 0; k < table_question.Count(); ++k)
                    {
                        table_question t_q = table_question.ElementAt(k);
                        workSheet.Cells[2, cols].Value = t_q.name;
                        rows = 3;
                        for (int j = 0; j < users.Count(); ++j)
                        {
                            user u = users.ElementAt(j);
                            List<answer_table> a3 = answer_table.Where(p => p.id_user == u.id && p.id_table_question == t_q.id).ToList();
                            string res = "";
                            for (int w = 0; w < a3.Count(); ++w)
                            {
                                res += a3.ElementAt(w).table_variant.name;
                                if (w < (a3.Count() - 1))
                                {
                                    res += ", ";
                                }
                            }
                            workSheet.Cells[rows++, cols].Value = res;
                        }
                        cols++;
                    }
                }
                else if (list.ElementAt(i).id_type_question == 7)
                {
                    for (int j = 0; j < users.Count(); ++j)
                    {
                        user u = users.ElementAt(j);
                        answer_file a4 = answer_file.FirstOrDefault(p => p.user_id == u.id && p.variant_file.question_id == q.id);
                        workSheet.Cells[rows++, cols].Value = (a4 != null) ? a4.link : "";
                    }
                    cols++;
                }
                
            }
            var path = Path.Combine(Server.MapPath("~/Content/files/"), "ask_" + form_id + "_"+ DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + path);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            FileInfo fi = new FileInfo(path);
            long sz = fi.Length;
            Response.ClearContent();
            Response.ContentType = Path.GetExtension(path);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(path)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(path);
            Response.End();

            System.IO.File.Delete(path);
            return Redirect("/Forms/Details?id=" + form_id);
        }

        //Результаты. Статистическая информация
        public ActionResult ExportStatistics(int form_id)
        {
            int cols = 1;
            List<answer_text> answer_text = db.answer_text.Where(p => p.question.id_form == form_id).ToList();
            List<answer_variant> answer_variant = db.answer_variant.Where(p => p.variant.question.id_form == form_id).ToList();
            List<answer_table> answer_table = db.answer_table.Where(p => p.table_question.question.id_form == form_id).ToList();
            List<answer_file> answer_file = db.answer_file.Where(p => p.variant_file.question.id_form == form_id).ToList();
            List<answer_else> answer_else = db.answer_else.Where(p => p.question_else.question.id_form == form_id).ToList();
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Статистическая информация");
            List<question> list = db.question.Where(p => p.id_form == form_id).ToList();
            List<user> users = db.user.Where(p => p.answer_text.Count(s => s.question.id_form == form_id) > 0 || p.answer_variant.Count(s => s.variant.question.id_form == form_id) > 0 || p.answer_table.Count(s => s.table_question.question.id_form == form_id) > 0 || p.answer_file.Count(s => s.variant_file.question.id_form == form_id) > 0).ToList();
            for (int i = 0; i < list.Count(); ++i)
            {
                question q = list.ElementAt(i);
                workSheet.Cells[1, cols].Value = list.ElementAt(i).name;
                if (list.ElementAt(i).id_type_question == 1)
                {
                    workSheet.Cells[4, cols].Value = answer_text.Count(p=>p.id_question==q.id);
                    cols++;
                }
                else if (list.ElementAt(i).id_type_question == 2 || list.ElementAt(i).id_type_question == 3 || list.ElementAt(i).id_type_question == 4)
                {
                    List<variant> a2 = db.variant.Where(p => p.id_question == q.id).ToList();
                    for(int j=0; j<a2.Count(); ++j)
                    {
                        variant v = a2.ElementAt(j);
                        workSheet.Cells[3, cols].Value = v.name;
                        workSheet.Cells[4, cols].Value = users.Count(p=>p.answer_variant.Count(d=>d.id_variant==v.id)>0);
                        cols++;
                    }
                    if (q.question_else.Count() > 0)
                    {
                        question_else q_e=db.question_else.First(p=>p.id_question==q.id);
                        workSheet.Cells[3, cols].Value = q_e.name;
                        workSheet.Cells[4, cols].Value = users.Count(p => p.answer_else.Count(d => d.id_question_else == q_e.id) > 0);
                        cols++;
                    }
                }
                else if (list.ElementAt(i).id_type_question == 5 || list.ElementAt(i).id_type_question == 6)
                {
                    List<table_question> table_question = db.table_question.Where(p => p.id_question == q.id).ToList();
                    for (int k = 0; k < table_question.Count(); ++k)
                    {
                        table_question t_q = table_question.ElementAt(k);
                        workSheet.Cells[2, cols].Value = t_q.name;
                        List<table_variant> t_V = db.table_variant.Where(p => p.id_question == t_q.id_question).ToList();
                        for (int l= 0; l < t_V.Count(); ++l)
                        {
                            table_variant t = t_V.ElementAt(l);
                            workSheet.Cells[3, cols].Value = t.name;
                            workSheet.Cells[4, cols].Value = users.Count(p => p.answer_table.Count(d => d.id_table_variant == t.id && d.id_table_question==t_q.id) > 0);
                            cols++;
                        }
                    }
                }
                else if (list.ElementAt(i).id_type_question == 7)
                {
                    workSheet.Cells[4, cols].Value = answer_file.Count(p => p.variant_file.question_id == q.id);
                    cols++;
                }

            }
            var path = Path.Combine(Server.MapPath("~/Content/files/"), "static_information_" + form_id + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + path);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            FileInfo fi = new FileInfo(path);
            long sz = fi.Length;
            Response.ClearContent();
            Response.ContentType = Path.GetExtension(path);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(path)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(path);
            Response.End();

            System.IO.File.Delete(path);
            return Redirect("/Forms/Details?id=" + form_id);
        }


        //Папка с прикрепленными файлами
        public ActionResult ExportZip(int form_id)
        {
            List<answer_file> files = db.answer_file.Where(p => p.variant_file.question.id_form == form_id).ToList();
            using (ZipFile zip = new ZipFile())
            {
               foreach(var file in files)
                {
                    zip.AddFile(Path.Combine(Server.MapPath(file.link),""));
                }
                var path = Path.Combine(Server.MapPath("~/Content/files/"), "files_" + form_id + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip");
                zip.Save(path);
            
                FileInfo fi = new FileInfo(path);
                long sz = fi.Length;
                Response.ClearContent();
                Response.ContentType = Path.GetExtension(path);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(path)));
                Response.AddHeader("Content-Length", sz.ToString("F0"));
                Response.TransmitFile(path);
                Response.End();
                System.IO.File.Delete(path);
            }
            return Redirect("/Forms/Details?id=" + form_id);
        }

        //Отображается после ответов на вопросы формы
        public ActionResult Send(int form_id)
        {
            form f = db.form.Find(form_id);
            return View(f);
        }


        //Страница для просмотра видео
        public ActionResult video(int id)
        {
            question q = db.question.Find(id);
            return View(q);
        }


    }
}
