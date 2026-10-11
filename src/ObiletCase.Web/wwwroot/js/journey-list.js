// Remembers the search of the journey page in the browser, so the search form can start with it on the next visit.
// It is saved here and not on the form, because a search that reached this page has passed the validation.

const journeySearch = document.getElementById('journeySearch');

const lastSearch = {
    originId: journeySearch.dataset.originId,
    originName: journeySearch.dataset.originName,
    destinationId: journeySearch.dataset.destinationId,
    destinationName: journeySearch.dataset.destinationName,
    departureDate: journeySearch.dataset.departureDate
};

// The same key is read by journey-search.js.
localStorage.setItem('lastJourneySearch', JSON.stringify(lastSearch));
