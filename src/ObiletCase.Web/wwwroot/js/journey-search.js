// Behaviour of the journey search form. The script is loaded at the end of the page, so the form already exists.

const originSelect = document.getElementById('OriginId');
const destinationSelect = document.getElementById('DestinationId');
const swapButton = document.getElementById('swapLocations');
const dateInput = document.getElementById('DepartureDate');
const dateShortcutButtons = document.querySelectorAll('.date-shortcut');

// The option lists are swapped together with the selected values,
// because the two lists can hold different locations after a search.
swapButton.addEventListener('click', function () {
    const originOptions = originSelect.innerHTML;
    const originValue = originSelect.value;
    const destinationOptions = destinationSelect.innerHTML;
    const destinationValue = destinationSelect.value;

    originSelect.innerHTML = destinationOptions;
    originSelect.value = destinationValue;

    destinationSelect.innerHTML = originOptions;
    destinationSelect.value = originValue;
});

// Marks the "Bugün" or "Yarın" button whose date is currently in the date field.
function highlightSelectedDateShortcut() {
    dateShortcutButtons.forEach(function (button) {
        if (button.dataset.date === dateInput.value) {
            button.classList.add('active');
        } else {
            button.classList.remove('active');
        }
    });
}

// The server writes each button's date into its data-date attribute, so no date is calculated here.
dateShortcutButtons.forEach(function (button) {
    button.addEventListener('click', function () {
        dateInput.value = button.dataset.date;
        highlightSelectedDateShortcut();
    });
});

dateInput.addEventListener('change', highlightSelectedDateShortcut);

highlightSelectedDateShortcut();
