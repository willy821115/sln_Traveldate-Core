///////////////////////////////////////////////////////���X�j��ñ�U���p����/////////////////////////////////////////////////////////////
$(".category-click").on('click', function () {
    const $arrowBottomDown = $(this).find("#arrow_bottom_down");
    const $arrowTopUp = $(this).find("#arrow_top_up");
    $(this).next(".filter-block-toggle").slideToggle(); //.drop_down_list_center �O�U�@�ӦP�Ť���
    $arrowBottomDown.toggle();
    $arrowTopUp.toggle();
});

//////////////////////////////////////////////////checkbox�����button�s��/////////////////////////////////////////////////////////////////
var isFirstButton = true;
var cleanAll = $('<button>', {
    text: 'X',
    value: "cleanAll",
    id: "btn-clean-all",
    class: "btn btn-outline-secondary mr-1",
    click: function () {
        $('#selected-checkboxes').empty(); // �M���������s
        isFirstButton = true; // ���]���Ĥ@���s�W���A
        $('.uncheckbox').show();
        $('.checkbox').hide();
        $('.calendar-container').find('.selected').remove();
    }
});

$('.uncheckbox').on('click', function () {
    if (isFirstButton) {
        $('#selected-checkboxes').append(cleanAll);
        isFirstButton = false;
    }
    
    var originalCheckbox= $(this).hide();
    var originalUncheckbox =$(this).next(".checkbox").show();
        var text = $(this).siblings("span").text(); // ���span������r
        var button = $('<button>', {
            text: text + ' X',
            value: text,
            class: "btn btn-outline-secondary mr-1",
            click: function () {
                $(this).remove();
                originalCheckbox.show(); // ��ܹ����� .checkbox
                originalUncheckbox.hide(); // ��ܹ����� .uncheckbox
                checkCleanAllButtonExistence();
            }
        });
    $('#selected-checkboxes').append(button); // �N���s�l�[��id��displayText��������
    if ($('#selected-checkboxes').children('button').length === 1) {
        $('#selected-checkboxes').empty(); // �M���������s
    }
    });

$('.checkbox').on('click', function () {
    if (isFirstButton) {
        $('#selected-checkboxes').append(cleanAll);
        isFirstButton = false;
    }
        $(this).hide();
        $(this).prev(".uncheckbox").show();
        var text = $(this).siblings("span").text(); // ���span������r
        var buttonsToRemove = $('#selected-checkboxes').find('button[value="' + text + '"]'); // �M��ݭn�R�������s
    buttonsToRemove.remove(); // �N���s�q #selected-checkboxes �R��
    checkCleanAllButtonExistence();
    });

function checkCleanAllButtonExistence() {
    if ($('#selected-checkboxes').children('button').length === 1) {
        $('#selected-checkboxes').empty(); // �M���������s
        isFirstButton = true;
    }
}



///////////////////////////////////////////����z��////////////////////////////////////////////////////////////

var first_click = true;
var date_string = '';
$('#calendar_first').on('click', function () {
    if ($('#selected-checkboxes').find('.date_button').length > 0) {
        $('.date_button').remove();
    }
    if (first_click) {
        var year_first = $(this).find('#year_f').text();
        var month_first = $(this).find('#month_f').text();
        var date_first = $(this).find('.selected').text();
        date_string = year_first + '/' + month_first + '/' + date_first;
        first_click = false;
    }
    else {        
       var year_second = $(this).find('#year_f').text();
       var month_second = $(this).find('#month_f').text();
        var date_second = $(this).find('.selected').last().text();
        first_click = true;
        date_string += '~' + year_second + '/' + month_second + '/' + date_second + '  X';
        var button = $('<button>', {
            text: date_string,// �]�w���s��r�� div ����r���e
            class: "btn btn-outline-secondary mr-1 date_button",
            click: function () {
                $(this).remove(); // �I�����s�ɧR���ӫ��s
                $('.calendar-container').find('.selected').remove();
            }
        });
        date_string = '';
        $('#selected-checkboxes').append(button); 
    }
});


$('#calendar_second').on('click', function () {
    if ($('#selected-checkboxes').find('.date_button').length > 0) {
        $('.date_button').remove();
    }
    if (first_click) {
        var year_first = $(this).find('#year_s').text();
        var month_first = $(this).find('#month_s').text();
        var date_first = $(this).find('.selected').text();
        date_string = year_first + '/' + month_first + '/' + date_first;
        first_click = false;
    }
    else {
        var year_second = $(this).find('#year_s').text();
        var month_second = $(this).find('#month_s').text();
        var date_second = $(this).find('.selected').last().text();
        first_click = true;
        date_string += '~' + year_second + '/' + month_second + '/' + date_second + 'X';
        var button = $('<button>', {
            text: date_string,// �]�w���s��r�� div ����r���e
            class: "btn btn-outline-secondary mr-1 date_button",
            click: function () {
                $(this).remove(); // �I�����s�ɧR���ӫ��s
                $('.calendar-container').find('.selected').remove();
            }
        });
        date_string = '';
        $('#selected-checkboxes').append(button);
    }
});
        

///////////////////////////////////////////�j�M����////////////////////////////////////////////////////////////
$('#btn_filter_price').on('click', function () {
    if ($('#selected-checkboxes').find('.price_button').length > 0) {
        $('.price_button').remove();
    }
    var first_price = $('.outputOne').text();
    var second_price = $('.outputTwo').text();
    var button = $('<button>', {
        text: '$  ' + first_price + '~' + '$  '+second_price+'  X',// �]�w���s��r�� div ����r���e
        class: "btn btn-outline-secondary mr-1 price_button",
        click: function () {
            $(this).remove(); // �I�����s�ɧR���ӫ��s
        }
    });
    $('#selected-checkboxes').append(button);
})
     









