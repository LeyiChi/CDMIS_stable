//document.write("<script type='text/javascript' src='d3.v3.js'></script>");

var colorseries = ["#d3a4ff", "#CA8EFF", "#BE77FF", "#B15BFF", "#9F35FF", "#921AFF", "#8600FF", "#6F00D2", "#5B00AE", "#3A006F"];

bordercolor = "#336699"; //提示窗口的边框颜色
titlecolor = "#99CCFF"; //提示窗口的标题颜色


function DrawMask(screenwidth, screenheight) {
    /*绘制蒙版---------------------------------------------------------------------------------------*/
    var mask = d3.select("body")
            .append("div")
            .attr("id", "maskDiv")
            .style("position", "absolute")
            .style("top", "0")
            .style("background", "#777")
            .style("filter", "progid:DXImageTransform.Microsoft.Alpha(style=3,opacity=25,finishOpacity=75")
            .style("opacity", "0.6")
            .style("left", "0")
            .style("width", screenwidth + "px")
            .style("height", (screenheight + 100) + "px")
            .style("z-index", "10000")
            .style("display", "block");
}

function DrawSubwindow(screenwidth, screenheight, subwindowwidth, subwindowheight, stitle) {
    /*绘制提示层---------------------------------------------------------------------------------------*/
    var marginLeft = (screenwidth - subwindowwidth) / 2;
    var marginTop = (screenheight - subwindowheight * 1.2) / 2;
    if (screenheight > 800) {
        marginTop = 200;
    }
    var subwindow = d3.select("body")
            .append("div")
            .attr("id", "subwindowDiv")
            .attr("align", "center")
            .style("background", "white")
            .style("border", "1px solid " + bordercolor)
            .style("position", "absolute")
            .style("left", marginLeft + "px")
            .style("top", marginTop + "px")
            .style("marginLeft", marginLeft + "px")
            .style("marginTop", marginTop + "px")
            .style("width", subwindowwidth + "px")
            .style("height", subwindowheight + "px")
            .style("z-index", "10001")

    /*绘制标题行---------------------------------------------------------------------------------------*/
    var title = subwindow.append("div")
            .attr("id", "Title");

    var table = title.append("table")
            .attr("width", "100%")
            .attr("height", "30px")
            .style("padding", "3px")
            .style("background", bordercolor)
            .style("border", "0");
    //console.log(stitle);
    var tr = table.append("tr")
    tr.append("td").attr("width", "80%")
            .append("h5")
            .attr("id", "deptname")
            .style("margin", "0")
            .style("font", "16px Verdana, Geneva, Arial, Helvetica, sans-serif")
            .style("font-weight", "bold")
            .style("color", "white")
            .text(stitle);

    tr.append("td")
            .append("h5")
            .attr("id", "close")
            .style("margin", "0")
            .attr("align", "right")

            .style("font", "14px Verdana, Geneva, Arial, Helvetica, sans-serif")
            .style("color", "white")
            .style("opacity", "0.75")
            .html("关闭")
            .style("cursor", "pointer").style("border", "0");

    title = document.getElementById("close");
    title.onclick = RemoveSubwindow;

    /*绘制内容-----------------------------------------------------------------------------------*/
    var iframecontainer = subwindow.append("div")
        .attr("align", "center")
        .attr("id", "framecontainer")
        .style("overflow", "auto")
        .style("height", (subwindowheight - 30) + "px")
        .style("width", "100%");
    return iframecontainer;
}

function RemoveSubwindow() {
    d3.select("#maskDiv").remove();
    d3.select("#subwindowDiv").remove();
}


function RemoveandRefresh() {
    d3.select("#maskDiv").remove();
    d3.select("#subwindowDiv").remove();
    location.reload();
}