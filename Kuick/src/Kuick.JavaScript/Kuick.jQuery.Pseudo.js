// Kuick.jQuery.Pseudo

// <tag class='pseudo-for' for='targetID'>
$(function () {
	$('.pseudo-for').on(clickEventName, function () {
		var x = $('#' + $(this).attr('for'));
		if (x) {
			var type = x.attr('type');
			if (type) {
				// checked
				if (type == 'radio') {
					x.prop('checked', true);
					x.trigger('change');
				}
				// toggle
				if (type == 'checkbox') {
					x.prop('checked', !(x.prop('checked')));
					x.trigger('change');
				}
			}
		}
	});
});
