﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SportsStore.Domain.Entities.Product>" %>
<div class="item"> 
    <h3><%: Model.Name %></h3> 
    <%: Model.Description %> 

    <% using (Html.BeginForm("AddToCart", "Cart")) { %>
        <%: Html.HiddenFor(x => x.ProductID) %>
        <%: Html.Hidden("returnUrl", Request.Url.PathAndQuery) %>
        <input type ="submit" value="+ Add to Cart" />
    <% } %>

    <h4><%: Model.Price.ToString("c")%></h4> 
</div>
