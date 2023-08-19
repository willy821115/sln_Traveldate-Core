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



function clearAllFiltered() {
    const cleanAllElement = $('.cleanAll');
    cleanAllElement.addClass('d-none')
    $('#selected-checkboxes').empty(); // �M���������s
    $('#selected-checkboxes').append(cleanAllElement)
    $('.uncheckbox').show();
    $('.checkbox').hide();
    $('.calendar-container').find('.selected').remove();
    isFirstButton = true; // ���]���Ĥ@���s�W���A
}

$('.divFiltered').on('click', '.uncheckbox', function () {
    if (isFirstButton) {
        $('.cleanAll').removeClass('d-none')
        isFirstButton = false;
    }
    var originalCheckbox = $(this).hide();
    var originalUncheckbox = $(this).next(".checkbox").show();
    var text = $(this).siblings("span").text(); // ���span������r
    $('#selected-tags').append(text)
    selectedTags.push(text); // �N��r�K�[��}�C��
    updateSelectedTags();
    var button = $('<button>', {
        text: text + ' X',
        value: text,
        class: "btn btn-outline-secondary mr-1",
        click: function () {
            $(this).remove();
            originalCheckbox.show(); // ��ܹ����� .checkbox
            originalUncheckbox.hide(); // ��ܹ����� .uncheckbox
            checkCleanAllButtonExistence();
            var index = selectedTags.indexOf(text); // ����r�b�}�C��������
            removeTagsFormArr(index)
        }
    });
    $('#selected-checkboxes').append(button); // �N���s�l�[��id��displayText��������
    checkCleanAllButtonExistence()
})
$('.divFiltered').on('click', '.checkbox', function(){
    if (isFirstButton) {
        $('.cleanAll').removeClass('d-none')
        isFirstButton = false;
    }
    $(this).hide();
    $(this).prev(".uncheckbox").show();
    var text = $(this).siblings("span").text(); // ���span������r
    var index = selectedTags.indexOf(text); // ����r�b�}�C��������
    removeTagsFormArr(index)
    var buttonsToRemove = $('#selected-checkboxes').find('button[value="' + text + '"]'); // �M��ݭn�R�������s
    buttonsToRemove.remove(); // �N���s�q #selected-checkboxes �R��
    checkCleanAllButtonExistence();
});
function removeTagsFormArr(index) {
    if (index > -1) {
        selectedTags.splice(index, 1); // �q�}�C���R���Ӥ�r
        updateSelectedTags(); // ��s��ܿ襤���Ҫ�����
    }
}

//�T�{�O�_�u���@��input�b�̭�
 function checkCleanAllButtonExistence() {
    if ($('#selected-checkboxes').children('button').length === 0) {
        $('.cleanAll').addClass('d-none');
         isFirstButton = true;
    }
}


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
            checkCleanAllButtonExistence()
        }
    });
    $('#selected-checkboxes').append(button);
    $('.cleanAll').removeClass('d-none');
})

//////////////////////////////////////////////�M��keyword�̪��r/////////////////////////////////////////////////////////////     
$('.clearKeyword').on('click', () => {
    $('#inputKeyword').val("")
    loadCities();
});








