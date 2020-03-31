 (function($) {

    $("#pricipal-slide").slider({
        range: "min",
        min: 5000,
        max: 5000000,
        value: 100000,
        step: 10000,
        slide: function(event, ui) {
            $("#pricipal").text(ui.value);
            loancalculate();
        }
    });
    $("#pricipal").text($("#pricipal-slide").slider("value"));

    $("#totalyear-slide").slider({
        range: "min",
        min: 3,
        max: 12,
        step: 1,
        value: 120,
        slide: function(event, ui) {
            $("#totalyear").text(ui.value);
            loancalculate();
        }
    });
    $("#totalyear").text($("#totalyear-slide").slider("value"));

    var year = $("#totalyear").text();
   
    //var Ten = GetInteresRate(year);
   
    //alert(year)
    loancalculate();
})(jQuery);

 function GetInteresRate(Tenure) {
    var IntersetRate = 0;
    $.ajax({
        type: "POST",
        url: "http://localhost:4264/AdminA/InterestRate",
        data: "{Tenure:'" + Tenure + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
           
            IntersetRate = data.replace(/^\D+/g, '').replace("}", "")
           
        },
        async:false
      
    });
    return IntersetRate;
 
 }

 //function getval(data)
 //{
 //    //alert('test');
 //    var InterestRate = data;//.replace(/^\D+/g, '').replace("}", "");
    
 //    alert(InterestRate);
 //    return InterestRate;
     
 //}

function loancalculate()
 {
    
    var year = $("#totalyear").text();  
    var rateOfInterest = GetInteresRate(year);
    var loanAmount = $("#pricipal").text();
    var numberOfMonths = $("#totalyear").text();
   
    var LAWNM = parseFloat(loanAmount * numberOfMonths);
    var MonthlyInt = parseFloat(parseFloat(rateOfInterest) * 0.01);
    var MonthlyIntRepayment = parseFloat(LAWNM * MonthlyInt);
    var TotalRepayment = (parseFloat(MonthlyIntRepayment) + parseFloat(loanAmount));
    var monthlyRepayment = (parseFloat(TotalRepayment) / parseFloat(numberOfMonths));
   
    var emi_str = monthlyRepayment.toFixed(2).toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
    var int_str = MonthlyIntRepayment.toFixed(2).toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    var full_str = TotalRepayment.toFixed(2).toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    



    $("#emi").html(emi_str);
    $("#tbl_emi").html(int_str);
    $("#tbl_la").html(full_str);


}