const toggleButton = document.getElementById('toggleButton');
const tripidContainer = document.getElementById('tripidContainer');

toggleButton.addEventListener('click', function () {
    if (tripidContainer.style.display === 'none') {
        tripidContainer.style.display = 'flex';
    } else {
        tripidContainer.style.display = 'none';
    }
});