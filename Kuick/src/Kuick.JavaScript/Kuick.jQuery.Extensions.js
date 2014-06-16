// Kuick.jQuery.Extensions

(function ($) {
	var DISPLAY = 'display';
	var BLOCK = 'block';
	var NONE = 'none';
	$.fn.extend({
		forceShow: function () { this.css(DISPLAY, BLOCK); },
		forceHide: function () { this.css(DISPLAY, NONE); },
		isShow: function () { return this.css(DISPLAY) == BLOCK; },
		isHide: function () { return this.css(DISPLAY) == NONE; },
		getCloset: function (tag) {
			var tags = this.parentsUntil(tag);
			var tag = tags.length == 0
				? x.parent()
				: $(tags[tags.length - 1]).parent();
			return tag;
		},
		attributeToQueryString: function () {
			var list = new Array();
			this.each(function () {
				$.each(this.attributes, function () {
					if (this.specified && this.name.indexOf('data-') == 0) {
						if (list.length > 0) { list.push('&'); }
						list.push(this.name.substr('data-'.length));
						list.push('=');
						list.push(encodeURIComponent(this.value));
					}
				});
			});
			return list.join('');
		},
	});
})(jQuery);


$(function () {
	var fun = null;
	var i = 0, len = window.Asynchronous.length;
	for (; i < len  ; i++) {
		fun = window.Asynchronous[i];
		fun.call();
	}
});


// Validation
window.Validations.prototype.clickToSubmit = function ($Object, operation) {
	operation = typeof (operation !== 'undefined') ? operation : {};
	$Object.click(function () {
		var $form = $(this).closest("form").eq(0);
		if ($form) { window.inputSubmit($form, opt); }
	});
};


