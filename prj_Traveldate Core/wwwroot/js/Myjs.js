
//    $(document).ready(function() {
//        $("#addButton").click(function () {
//            var photoInput = $("#photoInput")[0].files[0]; // 获取上传的照片文件
//            var description = $("#descriptionInput").val(); // 获取描述文本

//            // 创建一个新的 <div> 元素来容纳内容
//            var contentDiv = $("<div>").addClass("content-item");

//            // 创建一个 <img> 元素来显示上传的照片
//            var imgElement = $("<img>").attr("src", URL.createObjectURL(photoInput));
//            contentDiv.append(imgElement);

//            // 创建一个 <p> 元素来显示描述文本
//            var descriptionElement = $("<p>").text(description);
//            contentDiv.append(descriptionElement);

//            // 将新的内容添加到 contentContainer 中
//            $("#contentContainer").append(contentDiv);

//            // 清除输入框和文件选择框
//            $("#photoInput").val("");
//            $("#descriptionInput").val("");
//        });
//    });

//$(document).ready(function () {
//    $("#addButton").click(function () {
//        var photoInput = $("#photoInput")[0].files[0]; // 获取上传的照片文件
//        var description = $("#descriptionInput").val(); // 获取描述文本

//        // 创建一个新的 <div> 元素来容纳内容
//        var contentDiv = $("<div>").addClass("col-lg-12");

//        // 创建一个 <img> 元素来显示上传的照片
//        var imgElement = $("<img>").attr("src", URL.createObjectURL(photoInput)).addClass("tripimg");
//        contentDiv.append(imgElement);

//        // 创建一个 <p> 元素来显示描述文本
//        var descriptionElement = $("<p>").text(description).addClass();
//        contentDiv.append(descriptionElement);

//        // 将新的内容添加到 contentContainer 中
//        $("#contentContainer").append(contentDiv);

//        // 清除输入框和文件选择框
//        $("#photoInput").val("");
//        $("#descriptionInput").val("");
//    });
//});
//$(document).ready(function () {
//    // 選擇用戶名稱的 <span> 元素
//    var toggleButton = $(".toggle-button");

//    // 選擇聊天框的 <div> 元素
//    var chatBox = $(".chat_box");

//    // 選擇 dialogBox 的 <div> 元素
//    var dialogBox = $("#dialogBox");

//    var mailIcon = $("#mail");

//    // 初始高度
//    var originalHeight = dialogBox.height();

//    // 添加點擊事件處理程序
//    toggleButton.click(function () {
//        // 切換聊天框的顯示狀態
//        chatBox.slideToggle({
//            direction: "up", // 设置方向为向上展开
//            duration: 10
//        });
//        mailIcon.css("display", "none");

//        // 檢查 dialogBox 的當前高度
//        var currentHeight = dialogBox.height();

//        // 如果當前高度不等於原始高度，將高度設置為400px；否則恢復原始高度
//        if (currentHeight == originalHeight) {
//            dialogBox.height(400);
//        } else {
//            dialogBox.height(originalHeight);
//        }
//    });
//});




    
