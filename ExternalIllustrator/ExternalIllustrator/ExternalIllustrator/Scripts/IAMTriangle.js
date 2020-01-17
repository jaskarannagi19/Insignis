function famTriangleInit(Rate, Security, Ease) {
    famTriangleRefreshCanvas();
    famTriangleDrawOuterTriangle();
    famTriangleDrawInnerTriangle(Rate, Security, Ease);
}
		
function famTriangleUpdateDisplay()
{
    famTriangleRefreshCanvas();
    famTriangleDrawOuterTriangle();
			
    var Rate = document.getElementById("_rate").value;
    var Security = document.getElementById("_security").value;
    var Ease = document.getElementById("_ease").value;
			
    famTriangleDrawInnerTriangle(Rate, Security, Ease);
}
		
function famTriangleRefreshCanvas() {
    var canvas = document.getElementById("canvas");
    if(canvas.getContext){
        var ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, canvas.width, canvas.height);
    }
}
		
function famTriangleDrawOuterTriangle() {
    var canvas = document.getElementById("canvas");
    if(canvas.getContext){
        var ctx = canvas.getContext("2d");
        // OUTER TRIANGLE
        // line color
        ctx.fillStyle = 'rgba(0, 33, 56, 0.75)';
        ctx.strokeStyle = 'rgba(0, 33, 56, 0.75)';
        ctx.lineWidth = 3;
				
        ctx.beginPath();
        // Draw a triangle location for each corner from x:y 100,110 -> 200,10 -> 300,110 (it will return to first point)
        ctx.moveTo(0,300);
        ctx.lineTo(150,0);
        ctx.lineTo(300,300);
        ctx.stroke();
				
        ctx.beginPath();
        ctx.moveTo(0,300);
        //ctx.quadraticCurveTo(150, 250, 300, 300);
        ctx.lineTo(300, 300);
        ctx.stroke();				
    }
}

function famTriangleDrawInnerTriangle(Rate, Security, Ease) {
    var canvas = document.getElementById("canvas");
    if(canvas.getContext){
        var ctx = canvas.getContext("2d");

        // line color
        ctx.fillStyle = 'rgba(0, 33, 56, 0.75)';
        ctx.strokeStyle = 'rgba(0, 33, 56, 0.75)';
        ctx.lineWidth = 3;
        // INNER TRIANGLE
				
        var rateX = 150;
        var rateY = (150 - (30 * Rate));
				
        var easeX = (150 - (30 * Ease));
        var easeY = (150 + (30 * Ease));
				
        var securityX = (150 + (30 * Security));
        var securityY = (150 + (30 * Security));
				
        ctx.beginPath();
        ctx.moveTo(easeX, easeY);
        ctx.lineTo(rateX, rateY);
        ctx.lineTo(securityX, securityY);
        //ctx.closePath();
								
        ctx.moveTo(securityX,securityY);
        var ctrlPoint = 0;
        var ctrlStart = easeY;
        if(easeY < securityY){
            ctrlStart = easeY;
            ctrlPoint = easeY-securityY;
        }
        else{
            ctrlStart = securityY;
            ctrlPoint = securityY-easeY;
        }
        ctrlPoint -= 50;
        //ctx.quadraticCurveTo(150, ctrlStart+ctrlPoint, easeX, easeY);
        ctx.lineTo(150, ctrlStart + ctrlPoint);
        ctx.fill();
    }
}
