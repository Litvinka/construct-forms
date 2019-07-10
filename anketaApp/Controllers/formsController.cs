using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using anketaApp.Models;
using System.Text;
using System.Security.Cryptography;
using System.Data.Entity.Migrations;
using System.IO;

namespace anketaApp.Controllers
{
    public class formsController : Controller
    {
        private anketaEntities db = new anketaEntities();

        //Список всех форм
        public ActionResult Index()
        {
            List<form> form = db.form.OrderBy(p=>p.date_start).ToList();
            return View(form);
        }

        //Просмотр формы по идентификатору
        public ActionResult Details(int? id)
        {
            ViewBag.type_quest = new SelectList(db.type_question, "id", "name");
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

        //Создание формы
        public ActionResult Create()
        {
            ViewBag.id_state = new SelectList(db.state, "id", "name");
            return View();
        }

        //Создание новой анкеты (формы)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name, id_state,text_after, text_start, text_before_send")] form form)
        {
            if (ModelState.IsValid)
            {
                form.id = (db.form.Count() > 0) ? (db.form.Max(p => p.id) + 1) : 1;
                form.date_start = DateTime.Now;
                db.form.Add(form);
                db.SaveChanges();
                return Redirect("/forms/details?id="+form.id);
            }

            ViewBag.id_state = new SelectList(db.state, "id", "name", form.id_state);
            return View(form);
        }

        //Редактирование формы
        public ActionResult Edit(int? id)
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
            ViewBag.id_state = new SelectList(db.state, "id", "name", form.id_state);
            return View(form);
        }

        //Добавление нового вопроса в форму. В качестве параметров принимаются тип вопроса и идентификатор формы
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddQuestion(int type_q, int form_id)
        {
            question q = new question(); //Создание нового вопроса
            q.id = (db.question.Count() > 0) ? (db.question.Max(p => p.id) + 1) : 1; //идентификатор для нового вопроса
            q.id_form = form_id; //для какой формы создается вопрос
            q.id_type_question = type_q; //тип вопроса
            q.number = Convert.ToInt32(Request.Form["number_ask"]); //номер вопроса в форме
            q.name = Request.Unvalidated.Form["text_quest"]; //вопрос
            q.video_url = Request.Form["video"]; //ссылка на видео (если есть)
            q.is_required = (Convert.ToInt32(Request.Form["is_required"])==1) ? true : false; //обязательно или нет отвечать на вопрос
            var f = Request.Files["file"]; //прикреплен ли файл (картинка)
            if (f != null && f.ContentLength > 0) //если файл прикреплен и он не пустой
            {
                string newName = String.Format("File_{0}_{1}{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(), Path.GetExtension(f.FileName)); //изменение имени файла
                var fileName = Path.GetFileName(f.FileName); 
                var path = Path.Combine(Server.MapPath("~/Content/quest_file/"), newName); //путь для сохранения файла с новым именем
                f.SaveAs(path); //сохранение файла
                q.img_url = "/Content/quest_file/" + newName; //относительный путь к файлу
            }
            q.comment = Request.Unvalidated.Form["comment"]; //комментарий к вопросу
            db.question.Add(q); //добавление вопроса в бд
            db.SaveChanges(); //сохранение
            if (type_q == 2 || type_q == 3 || type_q == 4) //если тип вопроса от 2 до 4
            {
                string result = Request.Unvalidated.Form["variant"]; //список вариантов для ответа в одной строке через запятую
                String[] variant = result.Split(','); //преобразуем из строки в массив вариантов ответов
                List<variant> list_q = new List<variant>(); //для хранения вариантов ответа в экземплярах класса variant, для дальнейшего сохранения в бд
                int id_variant = (db.variant.Count() > 0) ? (db.variant.Max(p => p.id) + 1) : 1; //идентификатор варианта
                for (int i = 0; i < variant.Count(); ++i) //перебор вариантов ответов из массива
                {
                    if (!variant[i].Equals("")) //если вариант не пустой
                    {
                        variant quest = new variant(); //создание нового экземпляра класса variant
                        quest.id = id_variant++; //уникальный идентификатор
                        quest.name = variant[i].Replace("&#044", ",").Trim(); ; //вариант (текст)
                        quest.number = i + 1; //номер варианта
                        quest.id_question = q.id; //идентификатор вопроса, одним из возможных ответов которого может быть данный вариант
                        list_q.Add(quest); //добавление созданного и проинициализированного варианта в список
                    }
                }
                db.variant.AddRange(list_q); //добавления всех вариантов списка в бд
                db.SaveChanges(); //сохранение
                string else_v = Request.Unvalidated.Form["else"]; //вариант "другое"
                if (else_v != null) //если добавлен вариант "другой"
                {
                    question_else q_else = new question_else(); //создание экземпляра класса question_else
                    q_else.id = (db.question_else.Count() > 0) ? (db.question_else.Max(p => p.id) + 1) : 1; //идентификатор
                    q_else.id_question = q.id; //идентификатор вопроса, у коготого будет в вариантах ответа поле "другое"
                    q_else.name = else_v.Trim(); //название для поля "другое"
                    db.question_else.Add(q_else); //добавление в бд
                    db.SaveChanges(); //сохранение
                }
            }
            else if (type_q == 5 || type_q == 6) //если тип вопроса 5 или 6
            {
                string row = Request.Unvalidated.Form["table_row"]; //список вопросов в виде строки через запятую
                string[] rows = row.Split(','); //список вопросов в виде массива
                string col = Request.Unvalidated.Form["table_col"]; //список вариантов ответов через запятую
                string[] cols = col.Split(','); //список вариантов ответов в виде массива
                List<table_question> list_rows = new List<table_question>(); 
                int id = (db.table_question.Count() > 0) ? (db.table_question.Max(p => p.id) + 1) : 1;
                for (int i = 0; i < rows.Count(); ++i)
                {
                    table_question t_q = new table_question();
                    t_q.id = id++;
                    t_q.id_question = q.id;
                    t_q.number = i + 1;
                    t_q.name = rows[i].Replace("&#044", ",").Trim();
                    list_rows.Add(t_q);
                }
                List<table_variant> list_cols = new List<table_variant>();
                id = (db.table_variant.Count() > 0) ? (db.table_variant.Max(p => p.id) + 1) : 1;
                for (int i = 0; i < cols.Count(); ++i)
                {
                    table_variant t_v = new table_variant();
                    t_v.id = id++;
                    t_v.id_question = q.id;
                    t_v.number = i + 1;
                    t_v.name = cols[i].Replace("&#044", ",").Trim();
                    list_cols.Add(t_v);
                }
                db.table_question.AddRange(list_rows);
                db.table_variant.AddRange(list_cols);
                db.SaveChanges();
            }
            else if (type_q == 7) //если тип вопроса = 7
            {
                string row = Request.Form["file"]; 
                if (row != null)
                {
                    variant_file file = new variant_file();
                    file.id= (db.variant_file.Count() > 0) ? (db.variant_file.Max(p => p.id) + 1) : 1;
                    file.question_id = q.id;
                    file.max_size_file = Convert.ToInt32(row);
                    db.variant_file.Add(file);
                    db.SaveChanges();
                }
            }
            return Redirect("/forms/details?id=" + form_id+ "#addask");
        }


        //Удаление вопроса и ответов на него
        public ActionResult DeleteQuestion(int id)
        {
            question q = db.question.Find(id);
            int form_id = q.form.id;//идентификатор форм
            if (q.id_type_question == 1)
            {
                List<answer_text> list = db.answer_text.Where(p => p.id_question == q.id).ToList();
                db.answer_text.RemoveRange(list); //удаление ответов на текстовые вопросы
                db.SaveChanges();
            }
            else if (q.id_type_question == 2 || q.id_type_question==3 || q.id_type_question==4)
            {
                List<answer_variant> list1 = db.answer_variant.Where(p => p.variant.id_question == q.id).ToList();
                db.answer_variant.RemoveRange(list1);
                db.SaveChanges();
                List<variant> list = db.variant.Where(p => p.id_question == q.id).ToList();
                db.variant.RemoveRange(list);
                db.SaveChanges();
                if (db.answer_else.Count() > 0) //Удаление ответов в поле "другое"
                {
                    List<answer_else> list2 = db.answer_else.Where(p => p.question_else.id_question == q.id).ToList();
                    db.answer_else.RemoveRange(list2);
                    db.SaveChanges();
                }
                if (db.question_else.Count() > 0) //Удаление вопросов "другое"
                {
                    List<question_else> list2 = db.question_else.Where(p => p.id_question == q.id).ToList();
                    db.question_else.RemoveRange(list2);
                    db.SaveChanges();
                }
            }
            else if(q.id_type_question==5 || q.id_type_question == 6)
            {
                List<answer_table> list = db.answer_table.Where(p => p.table_question.id_question == q.id).ToList();
                db.answer_table.RemoveRange(list); //удаление ответов на вопросы в таблице
                db.SaveChanges();
                List<table_variant> list1 = db.table_variant.Where(p => p.id_question == q.id).ToList();
                db.table_variant.RemoveRange(list1); //удаление вариантов ответов из таблицы
                db.SaveChanges();
                List<table_question> list2 = db.table_question.Where(p => p.id_question == q.id).ToList();
                db.table_question.RemoveRange(list2); //удаление вопросов из таблицы
                db.SaveChanges();
            }
            else if (q.id_type_question == 7)
            {
                List<answer_file> list = db.answer_file.Where(p => p.variant_file.question_id == q.id).ToList();
                db.answer_file.RemoveRange(list); //удаление ссылок на загруженные файлы
                db.SaveChanges();
                List<variant_file> list1 = db.variant_file.Where(p => p.question_id == q.id).ToList();
                db.variant_file.RemoveRange(list1); //удаление вопросов, к которым нужно было прикреплять файлы в качестве ответа
                db.SaveChanges();
            }
            db.question.Remove(q);
            db.SaveChanges();
            return Redirect("/forms/details?id=" + form_id);
        }


        //Возвращает список районной выбранной области, преобразованный в .json
        [HttpPost]
        public JsonResult AllDistrict(string param)
        {
            int s = Convert.ToInt32(param); //Получение параметра "идентификатор области"
            var local = db.districts.Where(p => p.id_area == s); //Все районы заданной области
            List<List<String>> list = new List<List<String>>(); //Новый список для хранения значений (идентификатор района из бд, название района)
            List<String> row;
            foreach (districts p in local)
            {
                row = new List<String>();
                row.Add(Convert.ToString(p.id));
                row.Add(p.name);
                list.Add(row);
            } //Запись информации о районе в массив
            var d = System.Web.Helpers.Json.Encode(list); //преобразование списка, содержащего необходимую информацию о районах, в формат .json
            return Json(d);
        }

        //Возвращает список школ выбранного района, преобразованный в .json
        [HttpPost]
        public JsonResult AllSchool(string param)
        {
            int s = Convert.ToInt32(param);
            var local = db.schools.Where(p => p.id_district == s);
            List<List<String>> list = new List<List<String>>();
            List<String> row;
            foreach (schools p in local)
            {
                row = new List<String>();
                row.Add(Convert.ToString(p.id));
                row.Add(p.name);
                list.Add(row);
            }
            var d = System.Web.Helpers.Json.Encode(list);
            return Json(d);
        }

        //Страница редактирования вопроса
        public ActionResult EditQuestion(int id)
        {
            question question = db.question.Find(id);
            return View(question);
        }

        //Редактирование вопроса
        [HttpPost]
        public ActionResult EditQuestion(int id, string text_quest, int is_required, int number_ask, string comment)
        {
            question question = db.question.Find(id);
            question.name = text_quest;
            question.is_required = (is_required == 1) ? true : false;
            question.number = number_ask;
            question.comment = comment;
            question.video_url = Request.Form["video"];
            var f = Request.Files["file"];
            if (f != null && f.ContentLength > 0)
            {
                string newName = String.Format("File_{0}_{1}{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(), Path.GetExtension(f.FileName));
                var fileName = Path.GetFileName(f.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/quest_file/"), newName);
                f.SaveAs(path);
                question.img_url = "/Content/quest_file/" + newName;
            }
            else
            {
                question.img_url = null;
            }
            db.question.AddOrUpdate(question);
            db.SaveChanges();
            if (question.id_type_question == 2 || question.id_type_question == 3 || question.id_type_question == 4) { //RADIO, CHECKBOX, SELECT
                string result = Request.Unvalidated.Form["variant"];
                String[] variant = result.Split(','); //список вариантов в виде массива
                List<variant> v = db.variant.Where(p => p.id_question == id).ToList();
                for(int i = 0; i < variant.Count(); ++i)
                {
                    if (!variant[i].Equals(""))
                    {
                        int k = 0;
                        variant[i] = variant[i].Replace("&#044", ",");
                        foreach (var item in v)
                        {
                            if (item.name.CompareTo(variant[i]) == 0)
                            {
                                k = 1;
                                break;
                            }
                        }
                        if (k == 0)
                        {
                            variant quest = new variant();
                            quest.id = (db.variant.Count() > 0) ? (db.variant.Max(p => p.id) + 1) : 1;
                            quest.name = variant[i];
                            quest.number = i + 1;
                            quest.id_question = question.id;
                            db.variant.Add(quest);
                            db.SaveChanges();
                        }
                    }  
                }
                for(int j=0;j<v.Count();++j)
                {
                    int k = 0;
                    for (int i = 0; i < variant.Count(); ++i)
                    {
                        if (v.ElementAt(j).name.CompareTo(variant[i]) == 0)
                        {
                            k = 1;
                            break;
                        }
                    }
                    if (k == 0)
                    {
                        int var_id = v.ElementAt(j).id;
                        List<answer_variant> a = db.answer_variant.Where(p => p.variant.id==var_id).ToList();
                        db.answer_variant.RemoveRange(a);
                        db.SaveChanges();
                        variant del = v.ElementAt(j);
                        db.variant.Remove(del);
                        v.RemoveAt(j);
                        db.SaveChanges();
                        --j;
                    }
                }

                //Вариант ДРУГОЕ
                string else_v = Request.Form["else"]; 
                int db_else = db.question_else.Count(p => p.id_question == id);
                if (db_else > 0 && else_v == null)
                {
                    if (db.answer_else.Count(p => p.question_else.id_question == id) > 0)
                    {
                        List<answer_else> a = db.answer_else.Where(p => p.question_else.id_question == id).ToList();
                        db.answer_else.RemoveRange(a);
                        db.SaveChanges();
                    }
                    List<question_else> b = db.question_else.Where(p => p.id_question == id).ToList();
                    db.question_else.RemoveRange(b);
                    db.SaveChanges();
                }
                else if (db_else == 0 && else_v != null)
                {
                    question_else q_else = new question_else();
                    q_else.id = (db.question_else.Count() > 0) ? (db.question_else.Max(p => p.id) + 1) : 1;
                    q_else.id_question = question.id;
                    q_else.name = else_v;
                    db.question_else.Add(q_else);
                    db.SaveChanges();
                }
                else if(db_else > 0 && else_v !=null)
                {
                    question_else q_else = db.question_else.First(p => p.id_question == id);
                    q_else.name = else_v;
                    db.question_else.AddOrUpdate(q_else);
                    db.SaveChanges();
                }
                //Вариант ДРУГОЕ (end)
            }

            else if (question.id_type_question == 5 || question.id_type_question == 6) //TYPE TABLE_RADIO AND TABLE_CHECKBOX
            {
                //DEL TABLE ROW
                string row = Request.Unvalidated.Form["table_row"]; 
                string[] rows = row.Split(',');
                List<table_question> q = db.table_question.Where(p => p.id_question == question.id).ToList();
                for(int i = 0; i < rows.Count(); ++i)
                {
                    int k = 0;
                    foreach(var item in q)
                    {
                        if (item.name.CompareTo(rows[i]) == 0)
                        {
                            k = 1;
                            break;
                        }
                    }
                    if (k == 0)
                    {
                        table_question t_q = new table_question();
                        t_q.id = (db.table_question.Count() > 0) ? (db.table_question.Max(p => p.id) + 1) : 1; ;
                        t_q.id_question = question.id;
                        t_q.number = i + 1;
                        t_q.name = rows[i].Replace("&#044", ",");
                        db.table_question.Add(t_q);
                        db.SaveChanges();
                    }
                }
                for (int i = 0; i < q.Count(); ++i)
                {
                    int k = 0;
                    foreach (var item in rows)
                    {
                        string name = q.ElementAt(i).name;
                        if (item.CompareTo(name) == 0)
                        {
                            k = 1;
                            break;
                        }
                    }
                    if (k == 0)
                    {
                        int var_id = q.ElementAt(i).id;
                        List<answer_table> a = db.answer_table.Where(p => p.id_table_question == var_id).ToList();
                        db.answer_table.RemoveRange(a);
                        db.SaveChanges();
                        table_question del = q.ElementAt(i);
                        db.table_question.Remove(del);
                        q.RemoveAt(i);
                        db.SaveChanges();
                        --i;
                    }
                }
                //DEL TABLE ROW (END)

                //DEL TABLE COL
                string col = Request.Unvalidated.Form["table_col"];
                string[] cols = col.Split(',');
                List<table_variant> v = db.table_variant.Where(p => p.id_question == question.id).ToList();
                for (int i = 0; i < cols.Count(); ++i)
                {
                    int k = 0;
                    foreach (var item in v)
                    {
                        if (item.name.CompareTo(cols[i]) == 0)
                        {
                            k = 1;
                            break;
                        }
                    }
                    if (k == 0)
                    {
                        table_variant t_v = new table_variant();
                        t_v.id = (db.table_variant.Count() > 0) ? (db.table_variant.Max(p => p.id) + 1) : 1; ;
                        t_v.id_question = question.id;
                        t_v.number = i + 1;
                        t_v.name = cols[i].Replace("&#044", ",");
                        db.table_variant.Add(t_v);
                        db.SaveChanges();
                    }
                }
                for (int i = 0; i < v.Count(); ++i)
                {
                    int k = 0;
                    foreach (var item in cols)
                    {
                        string name = v.ElementAt(i).name;
                        if (item.CompareTo(name) == 0)
                        {
                            k = 1;
                            break;
                        }
                    }
                    if (k == 0)
                    {
                        int var_id = v.ElementAt(i).id;
                        List<answer_table> a = db.answer_table.Where(p => p.id_table_variant == var_id).ToList();
                        db.answer_table.RemoveRange(a);
                        db.SaveChanges();
                        table_variant del = v.ElementAt(i);
                        db.table_variant.Remove(del);
                        v.RemoveAt(i);
                        db.SaveChanges();
                        --i;
                    }
                }
                //DEL TABLE COL (END)
            }

            if (question.id_type_question == 7) //TYPE FILE
            {
                string row = Request.Form["file"];
                if (row != null)
                {
                    variant_file file = db.variant_file.First(p => p.question_id == question.id);
                    file.max_size_file = Convert.ToInt32(row);
                    db.variant_file.AddOrUpdate(file);
                    db.SaveChanges();
                }
            }

            return View(question);
        }


        public ActionResult _all_questions(int form_id)
        {
            List<question> questions = db.question.Where(p => p.id_form == form_id).OrderBy(p => p.number).ToList();
            return PartialView(questions);
        }


        public ActionResult _radio(int quest_id)
        {
            List<variant> variant = db.variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(variant);
        }


        public ActionResult _radio_edit(int quest_id)
        {
            List<variant> variant = db.variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(variant);
        }


        public ActionResult _checkbox(int quest_id)
        {
            List<variant> variant = db.variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(variant);
        }

        public ActionResult _checkbox_edit(int quest_id)
        {
            List<variant> variant = db.variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(variant);
        }


        public ActionResult _select(int quest_id)
        {
            List<variant> variant = db.variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(variant);
        }

        public ActionResult _select_edit(int quest_id)
        {
            List<variant> variant = db.variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(variant);
        }


        public ActionResult _table_checkbox(int quest_id)
        {
            List<table_question> quest = db.table_question.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            ViewBag.cols = db.table_variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(quest);
        }

        public ActionResult _table_checkbox_edit(int quest_id)
        {
            List<table_question> quest = db.table_question.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            ViewBag.cols = db.table_variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(quest);
        }


        public ActionResult _table_radio(int quest_id)
        {
            List<table_question> quest = db.table_question.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            ViewBag.cols = db.table_variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(quest);
        }

        public ActionResult _table_radio_edit(int quest_id)
        {
            List<table_question> quest = db.table_question.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            ViewBag.cols = db.table_variant.Where(p => p.id_question == quest_id).OrderBy(p => p.number).ToList();
            return PartialView(quest);
        }


        public ActionResult _questions_school(int form_id)
        {
            ViewBag.area = new SelectList(db.areas, "id", "name");
            ViewBag.district = new SelectList(db.districts, "id", "name");
            ViewBag.school = new SelectList(db.schools, "id", "name");
            ViewBag.subject = new SelectList(db.subjects, "id", "name");
            return PartialView();
        }

        //Удаление формы (включает удаление всех вопросов с вариантами ответов и ответов на них)
        public ActionResult DelForm(int id)
        {
            form f = db.form.Find(id);

            //DELETE ANSWERS
            List<answer_text> a = db.answer_text.Where(p=>p.question.id_form==id).ToList();
            db.answer_text.RemoveRange(a);
            List<answer_variant> b = db.answer_variant.Where(p=>p.variant.question.id_form==id).ToList();
            db.answer_variant.RemoveRange(b);
            List<answer_else> c = db.answer_else.Where(p=>p.question_else.question.id_form==id).ToList();
            db.answer_else.RemoveRange(c);
            List<answer_table> d = db.answer_table.Where(p=>p.table_variant.question.id_form==id).ToList();
            db.answer_table.RemoveRange(d);
            List<answer_file> e = db.answer_file.Where(p=>p.variant_file.question.id_form==id).ToList();
            db.answer_file.RemoveRange(e);
            db.SaveChanges();
            //DELETE ANSWERS (END)

            //DELETE VARIANT
            List<variant> a1 = db.variant.Where(p => p.question.id_form == id).ToList();
            db.variant.RemoveRange(a1);
            List<variant_file> a2 = db.variant_file.Where(p => p.question.id_form == id).ToList();
            db.variant_file.RemoveRange(a2);
            List<table_variant> a3 = db.table_variant.Where(p=>p.question.id_form==id).ToList();
            db.table_variant.RemoveRange(a3);
            db.SaveChanges();
            //DELETE VARIANT (END)

            //DELETE QUESTION
            List<table_question> b1 = db.table_question.Where(p=>p.question.id_form==id).ToList();
            db.table_question.RemoveRange(b1);
            List<question_else> b2 = db.question_else.Where(p => p.question.id_form == id).ToList();
            db.question_else.RemoveRange(b2);
            List<question> b3 = db.question.Where(p => p.id_form == id).ToList();
            db.question.RemoveRange(b3);
            db.SaveChanges();
            //DELETE QUESTION (END)

            db.form.Remove(f);
            db.SaveChanges();
            return Redirect("/Forms/Index");
        }

        //Редактирование параметров формы
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(form form)
        {
            string state = Request.Form["state"]; //Состояние (могут ли пользователи отвечать на вопросы анкеты)
            form.id_state = (state != null) ? Convert.ToInt32(state) : 2;
            db.form.AddOrUpdate(form);
            db.SaveChanges();
            return Redirect("/forms/details?id=" + form.id);
        }

    }
}
