DemandOfferDonut(1);

var donutDatas;

function DemandOfferDonut(auid) {
    $.ajax({
        url: '/Report/DemandDonut?auid=' + auid,
        type: 'GET',
        success: function (result) {
            donutDatas = result;
            var donutl = new Morris.Donut({
                element: 'demand-chart',
                resize: true,
                colors: ["#3c8dbc", "#f56954", "#00a65a"],
                data: result,
                hideHover: 'auto'
            });


            donut();
        },
        error: function () {

        }
    });

    $.ajax({
        url: '/Report/OfferDonut?auid='+auid,
        type: 'GET',
        success: function (result) {
            var donut = new Morris.Donut({
                element: 'offer-chart',
                resize: true,
                colors: ["#3c8dbc", "#f56954", "#00a65a"],
                data: result,
                hideHover: 'auto'
            });
        },
        error: function () {

        }
    });
};



function donut()
{
   
    //var str=JSON.stringify(donutDatas);
    //var res = str.replace("value", "data");
    //alert(res);

    //console.log(donutDatas);
    var donutData = [
  { label: "Kartof", data: 50 },
  { label: "Soğan", data: 20 },
  { label: "Xiyar", data: 30 },
  { label: "Üzüm", data: 15 },
  { label: "Banan", data: 21 }
    ];

    $.plot("#donut-chart", donutData, {
        series: {
            pie: {
                show: true,
                radius: 1,
                innerRadius: 0.5,
                label: {
                    show: true,
                    radius: 2 / 3,
                    formatter: labelFormatter,
                    threshold: 0.1
                }

            }
        },
        legend: {
            show: false
        }
    });
}

/*
 * Custom Label formatter
 * ----------------------
 */
function labelFormatter(label, series) {
    return '<div style="font-size:13px; text-align:center; padding:2px; color: #fff; font-weight: 600;">'
        + label
        + "<br>"
        + Math.round(series.percent) + "%</div>";
}