///////////////////////////////////////////////////////收合大標簽下的小標籤/////////////////////////////////////////////////////////////
function toggleBlock() {
    const $arrowBottomDown = $(this).find("#arrow_bottom_down");
    const $arrowTopUp = $(this).find("#arrow_top_up");
    $(this).next(".filter-block-toggle").slideToggle(); //.drop_down_list_center 是下一個同級元素
    $arrowBottomDown.toggle();
    $arrowTopUp.toggle();
}

$(".filter-block-toggle").hide();
$(".category-click").on('click', toggleBlock);

//////////////////////////////////////////////////checkbox跟標籤button連動/////////////////////////////////////////////////////////////////
var isFirstButton = true;



function clearAllFiltered() {
    const cleanAllElement = $('.cleanAll');
    cleanAllElement.addClass('d-none')
    $('#selected-checkboxes').empty(); // 清除全部按鈕
    $('#selected-checkboxes').append(cleanAllElement)
    $('.uncheckbox').show();
    $('.checkbox').hide();
    $('.calendar-container').find('.selected').removeClass('selected');
    selectedCities = [];
    selectedTags = [];
    selectedTagsId = [];
    pageNumber = 1;
    $('#status').text("");
    //selectedTypes = [];
    //startTime = ""
    //endTime = ""
    //minPrice = ""
    //maxPrice = ""
    filteredByConditions()
    isFirstButton = true; // 重設為第一次新增狀態
}

$('.divFiltered_region').on('click', '.uncheckbox', function () {
    if (isFirstButton) {
        $('.cleanAll').removeClass('d-none')
        isFirstButton = false;
    }
    pageNumber = 1;
    var originalCheckbox = $(this).hide();
    var originalUncheckbox = $(this).next(".checkbox").show();
    var text = $(this).siblings("span").text(); // 獲取span中的文字
    selectedCities.push(text); // 將文字添加到陣列中

    filteredByConditions();
    var button = $('<button>', {
        text: text + ' X',
        value: text,
        class: "btn btn-outline-secondary mr-1",
        click: function () {
            $(this).remove();
            originalCheckbox.show(); // 顯示對應的 .checkbox
            originalUncheckbox.hide(); // 顯示對應的 .uncheckbox
            checkCleanAllButtonExistence();
            var index = selectedCities.indexOf(text); // 找到文字在陣列中的索引
            removeCitiesFormArr(index)
        }
    });
    $('#selected-checkboxes').append(button); // 將按鈕追加到id為displayText的元素中
    checkCleanAllButtonExistence()
})
$('.divFiltered_tag').on('click', '.uncheckbox', function () {
    if (isFirstButton) {
        $('.cleanAll').removeClass('d-none')
        isFirstButton = false;
    }
    pageNumber = 1;
    var originalCheckbox = $(this).hide();
    var originalUncheckbox = $(this).next(".checkbox").show();
    var text = $(this).siblings("span").text(); // 獲取span中的文字
    selectedTags.push(text); // 將文字添加到陣列中
    var tagId = $(this).siblings("span").attr('id');
    selectedTagsId.push(tagId);
    filteredByConditions();
    var button = $('<button>', {
        text: text + ' X',
        id: tagId,
        value: text,
        class: "btn btn-outline-secondary mr-1",
        click: function () {
            $(this).remove();
            originalCheckbox.show(); // 顯示對應的 .checkbox
            originalUncheckbox.hide(); // 顯示對應的 .uncheckbox
            checkCleanAllButtonExistence();
            var index = selectedTags.indexOf(text); // 找到文字在陣列中的索引
            var indexId = selectedTagsId.indexOf(tagId);
            removeTagsFormArr(index, indexId);
        }
    });
    $('#selected-checkboxes').append(button); // 將按鈕追加到id為displayText的元素中
    checkCleanAllButtonExistence()
})



$('.divFiltered_region').on('click', '.checkbox', function () {
    if (isFirstButton) {
        $('.cleanAll').removeClass('d-none')
        isFirstButton = false;
    }
    pageNumber = 1;
    $(this).hide();
    $(this).prev(".uncheckbox").show();
    var text = $(this).siblings("span").text(); // 獲取span中的文字
    var index = selectedCities.indexOf(text); // 找到文字在陣列中的索引
    
    removeCitiesFormArr(index)
    var buttonsToRemove = $('#selected-checkboxes').find('button[value="' + text + '"]'); // 尋找需要刪除的按鈕
    buttonsToRemove.remove(); // 將按鈕從 #selected-checkboxes 刪除
    checkCleanAllButtonExistence();
});
$('.divFiltered_tag').on('click', '.checkbox', function () {
    if (isFirstButton) {
        $('.cleanAll').removeClass('d-none')
        isFirstButton = false;
    }
    $(this).hide();
    $(this).prev(".uncheckbox").show();
    var text = $(this).siblings("span").text(); // 獲取span中的文字
    var tagId = $(this).siblings("span").attr('id');// 獲取span中的id
    var index = selectedTags.indexOf(text); // 找到文字在陣列中的索引
    var indexId = selectedTagsId.indexOf(tagId);
    removeTagsFormArr(index, indexId)
    var buttonsToRemove = $('#selected-checkboxes').find('button[value="' + text + '"]'); // 尋找需要刪除的按鈕
    buttonsToRemove.remove(); // 將按鈕從 #selected-checkboxes 刪除
    checkCleanAllButtonExistence();
});




function removeTagsFormArr(index, indexId) {
    if (index > -1) {
        selectedTags.splice(index, 1); // 從陣列中刪除該文字
        selectedTagsId.splice(indexId, 1);
        pageNumber = 1;
        filteredByConditions(); // 更新顯示選中標籤的元素
    }

}
function removeCitiesFormArr(index) {
    if (index > -1) {
        selectedCities.splice(index, 1); // 從陣列中刪除該文字
        pageNumber = 1;
        filteredByConditions(); // 更新顯示選中標籤的元素
    }
}


//確認是否只有一個input在裡面
function checkCleanAllButtonExistence() {
    if ($('#selected-checkboxes').children('button').length === 0) {
        $('.cleanAll').addClass('d-none');
        isFirstButton = true;
    }
}


///////////////////////////////////////////搜尋價格////////////////////////////////////////////////////////////
//$('#btn_filter_price').on('click', function () {
//    if ($('#selected-checkboxes').find('.price_button').length > 0) {
//        $('.price_button').remove();
//    }
//    var format_first_price = $('.outputOne').text();
//    var format_second_price = $('.outputTwo').text();
//    minPrice = format_first_price.replace(/[^0-9.-]+/g, "")
//    maxPrice = format_second_price.replace(/[^0-9.-]+/g, "")
//    filteredByConditions();
//    var button = $('<button>', {
//        text: '$  ' + format_first_price + '~' + '$  ' + format_second_price + '  X',// 設定按鈕文字為 div 的文字內容
//        class: "btn btn-outline-secondary mr-1 price_button",
//        click: function () {
//            $(this).remove(); // 點擊按鈕時刪除該按鈕
//            checkCleanAllButtonExistence()
//            minPrice = ""
//            maxPrice = ""
//            filteredByConditions()
//        }
//    });
//    $('#selected-checkboxes').append(button);
//    $('.cleanAll').removeClass('d-none');
//})

//////////////////////////////////////////////清除keyword裡的字/////////////////////////////////////////////////////////////     
//$('.clearKeyword').on('click', () => {
//    $('#inputKeyword').val("")
//    loadCities();
//});








