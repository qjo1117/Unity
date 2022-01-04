using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    // https://www.youtube.com/watch?v=uQZFawccnNg 고라니 TV 참고
    // 화면 비율을 고정으로 바꾸는 방법

    // 핸드폰의 화면이 고정적이지않아서 가장 자주쓰이는 화면비율을 기준으로 작업을 시작한다.

    void Start()
    {
        Camera camera = GetComponent<Camera>();      // 현재의 카메라를 가져옴.
        Rect rect = camera.rect;                        //(9.6f/16.0f)
        float scaleHeight = (((float)Screen.width/Screen.height));       // 현재 플레이어의 해상도를 먼저 가져오고 비율로 나눠버린다.
        float scaleWidth = 1.0f / scaleHeight;

        if(scaleHeight < 1.0f) {                    // height 큰 경우 (날씬한 경우)
            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
		}
        else {                                      // width가 큰 경우 (뚱뚱한 경우)
            rect.width = scaleWidth;
            rect.x = (1.0f - scaleWidth) / 2.0f;
		}
        
        // 설정한 값을 적용해준다.
        camera.rect = rect;
    }

    void Update()
    {
        
    }
}
