//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP {
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;
using System.Web.UI;
using System.Web.WebPages;
using System.Web.WebPages.Html;

#line 2 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
using PagedList.Mvc;

#line default
#line hidden

public class _Page_Index_cshtml : System.Web.WebPages.WebPage {
private static object @__o;
#line hidden
public _Page_Index_cshtml() {
}
protected System.Web.HttpApplication ApplicationInstance {
get {
return ((System.Web.HttpApplication)(Context.ApplicationInstance));
}
}
public override void Execute() {

#line 1 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
__o = model;


#line default
#line hidden

#line 3 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
  
    ViewBag.Title = "Index";
    Layout = "~/Views/Shared/_ManageStudentHome.cshtml";


#line default
#line hidden

#line 4 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
        if (Session["Role"].ToString() == "Admin")
        {
            using (Html.BeginForm())
            {
                

#line default
#line hidden

#line 5 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
               __o = Html.TextBox("SearchString", null, new { @class = "search-input", placeholder = "Nh?p t�n c?n t�m..." });


#line default
#line hidden

#line 6 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                      
            }
            

#line default
#line hidden

#line 7 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
       __o = Html.ActionLink("Xu?t Excel", "ExportToExcel", null, new { @class = "btnAdd" });


#line default
#line hidden

#line 8 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                                                                                            
            

#line default
#line hidden

#line 9 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
       __o = Html.ActionLink("+ Th�m L?p", "Create", null, new { @class = "btnAdd" });


#line default
#line hidden

#line 10 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                                                                                     
        }

#line default
#line hidden

#line 11 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
        if (Session["Role"].ToString() == "Student")
        {
            

#line default
#line hidden

#line 12 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
       __o = Html.ActionLink("+ ??ng K�", "Create", null, new { @class = "btnAdd" });


#line default
#line hidden

#line 13 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                                                                                    
        }

#line default
#line hidden

#line 14 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
            if (Session["Role"].ToString() == "Admin")
            {
                

#line default
#line hidden

#line 15 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
               __o = Html.ActionLink("T�n Sinh Vi�n", "Index", new { sortOrder = ViewBag.NameSortParm });


#line default
#line hidden

#line 16 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                     
            }

#line default
#line hidden

#line 17 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
           __o = Html.ActionLink("Ng�y ??ng K�", "Index", new { sortOrder = ViewBag.DateSortParm });


#line default
#line hidden

#line 18 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
        if (Model != null && Model.Any())
        {
            foreach (var item in Model)
            {
                

#line default
#line hidden

#line 19 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                    if (Session["Role"].ToString() == "Admin")
                    {
                        

#line default
#line hidden

#line 20 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                       __o = Html.DisplayFor(modelItem => item.Hoc_sinh.ten_sinh_vien);


#line default
#line hidden

#line 21 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                             
                    }

#line default
#line hidden

#line 22 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.DisplayFor(modelItem => item.ngay_dk);


#line default
#line hidden

#line 23 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.DisplayFor(modelItem => item.Lop_hoc_phan.ma_hoc_phan);


#line default
#line hidden

#line 24 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.DisplayFor(modelItem => item.Lop_hoc_phan.Lop_chinh.ten_lop);


#line default
#line hidden

#line 25 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.DisplayFor(modelItem => item.Lop_hoc_phan.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien);


#line default
#line hidden

#line 26 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.DisplayFor(modelItem => item.Lop_hoc_phan.Mon_hoc.ten_mon);


#line default
#line hidden

#line 27 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.ActionLink("Edit", "Edit", new { ma_sinh_vien = item.ma_sinh_vien, ma_hoc_phan = item.ma_hoc_phan });


#line default
#line hidden

#line 28 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.ActionLink("Details", "Details", new { ma_sinh_vien = item.ma_sinh_vien, ma_hoc_phan = item.ma_hoc_phan });


#line default
#line hidden

#line 29 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                   __o = Html.ActionLink("Delete", "Delete", new { ma_sinh_vien = item.ma_sinh_vien, ma_hoc_phan = item.ma_hoc_phan });


#line default
#line hidden

#line 30 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                     
            }
        }
        else
        {
            

#line default
#line hidden

#line 31 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                 
        }

#line default
#line hidden

#line 32 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
if (Model?.Any() == true)

{
    

#line default
#line hidden

#line 33 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                                   __o = Model.PageCount < Model.PageNumber ? 0 : Model.PageNumber;


#line default
#line hidden

#line 34 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
                                                                                                 __o = Model.PageCount;


#line default
#line hidden

#line 35 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
   __o = Html.PagedListPager(Model, page => Url.Action("Index", new { page, size = ViewBag.currentSize }));


#line default
#line hidden

#line 36 "C:\Users\trant\AppData\Local\Temp\0ED1446DBF6C8E0863A16A57A8FA7DE24A58\2\ManageStudentsV2-master\ManageStudentsV2\Views\Lop_dang_ky\Index.cshtml"
          
}

#line default
#line hidden
}
}
}
