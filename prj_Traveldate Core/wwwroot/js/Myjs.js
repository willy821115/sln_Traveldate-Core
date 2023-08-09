
    $(document).ready(function() {
        $("#addButton").click(function () {
            var photoInput = $("#photoInput")[0].files[0]; // 获取上传的照片文件
            var description = $("#descriptionInput").val(); // 获取描述文本

            // 创建一个新的 <div> 元素来容纳内容
            var contentDiv = $("<div>").addClass("content-item");

            // 创建一个 <img> 元素来显示上传的照片
            var imgElement = $("<img>").attr("src", URL.createObjectURL(photoInput));
            contentDiv.append(imgElement);

            // 创建一个 <p> 元素来显示描述文本
            var descriptionElement = $("<p>").text(description);
            contentDiv.append(descriptionElement);

            // 将新的内容添加到 contentContainer 中
            $("#contentContainer").append(contentDiv);

            // 清除输入框和文件选择框
            $("#photoInput").val("");
            $("#descriptionInput").val("");
        });
    });

$(document).ready(function () {
    $("#addButton").click(function () {
        var photoInput = $("#photoInput")[0].files[0]; // 获取上传的照片文件
        var description = $("#descriptionInput").val(); // 获取描述文本

        // 创建一个新的 <div> 元素来容纳内容
        var contentDiv = $("<div>").addClass("col-lg-12");

        // 创建一个 <img> 元素来显示上传的照片
        var imgElement = $("<img>").attr("src", URL.createObjectURL(photoInput)).addClass("tripimg");
        contentDiv.append(imgElement);

        // 创建一个 <p> 元素来显示描述文本
        var descriptionElement = $("<p>").text(description).addClass();
        contentDiv.append(descriptionElement);

        // 将新的内容添加到 contentContainer 中
        $("#contentContainer").append(contentDiv);

        // 清除输入框和文件选择框
        $("#photoInput").val("");
        $("#descriptionInput").val("");
    });
});





    
