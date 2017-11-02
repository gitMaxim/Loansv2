$.validator.methods.number = function (value, element) {
    return this.optional(element) || $.isNumeric($(element).val().replace('/', '.'));
}