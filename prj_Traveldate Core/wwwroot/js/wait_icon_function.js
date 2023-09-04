function showLoadingIcon() {
    $('.loading-icon').removeClass('d-none')
    $('#total').addClass('d-none')
    $('#filteredBody').addClass('d-none')
    $('.alert-light').removeClass('d-none')
}
function hideLoadingIcon() {
    $('.loading-icon').addClass('d-none')
    $('#total').removeClass('d-none')
    $('#filteredBody').removeClass('d-none')
    $('.alert-light').addClass('d-none')
}