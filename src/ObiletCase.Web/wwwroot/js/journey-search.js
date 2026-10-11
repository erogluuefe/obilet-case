// Behaviour of the journey search form. The script is loaded at the end of the page, so the form already exists.

const originField = document.getElementById('originField');
const destinationField = document.getElementById('destinationField');
const locationFields = document.querySelectorAll('.location-field');
const swapButton = document.getElementById('swapLocations');
const dateInput = document.getElementById('DepartureDate');
const dateShortcutButtons = document.querySelectorAll('.date-shortcut');

// How long the visitor has to pause typing before a location search is sent.
const searchDelayMilliseconds = 450;

// Writes a chosen location into a field: the id and the name go to the hidden inputs, the name is shown in the search box.
function selectLocation(field, id, name) {
    field.querySelector('.location-id').value = id;
    field.querySelector('.location-name').value = name;
    field.querySelector('.location-search').value = name;
}

swapButton.addEventListener('click', function () {
    const originId = originField.querySelector('.location-id').value;
    const originName = originField.querySelector('.location-name').value;
    const destinationId = destinationField.querySelector('.location-id').value;
    const destinationName = destinationField.querySelector('.location-name').value;

    selectLocation(originField, destinationId, destinationName);
    selectLocation(destinationField, originId, originName);
});

// Asks our own backend for the locations matching the typed text and shows them in the list under the search box.
async function searchLocations(searchInput, menu) {
    const searchText = searchInput.value;
    const url = searchInput.dataset.searchUrl + '?text=' + encodeURIComponent(searchText);

    const response = await fetch(url);

    // When the search fails the list is left as it is, so the visitor can still pick a location.
    if (!response.ok) {
        return;
    }

    const locations = await response.json();

    // The text changed while this answer was on its way, so the answer no longer belongs to what is in the box.
    if (searchInput.value !== searchText) {
        return;
    }

    menu.innerHTML = '';

    locations.forEach(function (location) {
        const item = document.createElement('button');
        item.type = 'button';
        item.className = 'dropdown-item';
        item.dataset.id = location.id;
        item.textContent = location.name;

        menu.appendChild(item);
    });
}

// Bootstrap opens and closes the list and handles the arrow keys; the code below only searches and selects.
locationFields.forEach(function (field) {
    const nameInput = field.querySelector('.location-name');
    const searchInput = field.querySelector('.location-search');
    const menu = field.querySelector('.dropdown-menu');
    const defaultLocations = menu.innerHTML;
    let waitTimer;

    // The box is emptied so the visitor can type straight away; the selected name stays visible as the placeholder.
    // Selecting the text instead would open the copy/paste menu on touch screens.
    // The list goes back to the locations the page was opened with, so an empty box never shows an old search.
    searchInput.addEventListener('focus', function () {
        searchInput.placeholder = nameInput.value;
        searchInput.value = '';
        menu.innerHTML = defaultLocations;
    });

    // Leaving the box without a choice puts the selected name back and drops a search that is still waiting.
    searchInput.addEventListener('blur', function () {
        clearTimeout(waitTimer);
        searchInput.value = nameInput.value;
    });

    // The search starts when the visitor pauses typing, so not every key press sends a request.
    searchInput.addEventListener('input', function () {
        bootstrap.Dropdown.getOrCreateInstance(searchInput).show();

        clearTimeout(waitTimer);

        waitTimer = setTimeout(function () {
            searchLocations(searchInput, menu);
        }, searchDelayMilliseconds);
    });

    // Enter in the search box would submit the form before a location is picked.
    searchInput.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault();
        }
    });

    // The click is listened to on the list, because its items are replaced after every search.
    menu.addEventListener('click', function (event) {
        const item = event.target.closest('.dropdown-item');

        if (item === null) {
            return;
        }

        selectLocation(field, item.dataset.id, item.textContent);

        // On phones the box keeps the focus after a tap on the list, so it is released here.
        searchInput.blur();
    });
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

// Starts the form with the last search the visitor made; journey-list.js saves it under the same key.
function restoreLastSearch() {
    // After a rejected search the form shows what the visitor just sent next to the error messages; that is left alone.
    if (document.querySelector('.field-validation-error') !== null) {
        return;
    }

    const savedSearch = localStorage.getItem('lastJourneySearch');

    if (savedSearch === null) {
        return;
    }

    const lastSearch = JSON.parse(savedSearch);

    selectLocation(originField, lastSearch.originId, lastSearch.originName);
    selectLocation(destinationField, lastSearch.destinationId, lastSearch.destinationName);

    // A day that has passed cannot be searched any more, so the default date of the page is kept.
    // The dates are written as year-month-day, so comparing them as text gives the right order.
    if (lastSearch.departureDate >= dateInput.min) {
        dateInput.value = lastSearch.departureDate;
    }
}

restoreLastSearch();
highlightSelectedDateShortcut();
