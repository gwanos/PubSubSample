# PubSubSample
비동기 메세지 '발행-구독' 모델 샘플

## Overview

- Subscriber: Console App
  - 구독자는 구독중인 채널로 발행되는 메세지를 수신한다.
  - 하나의 스레드는 한 명의 구독자를 의미한다.
- Publisher: Azure Function  
  - 발행자는 HTTP 요청을 받아 채널에 메세지를 발행한다.      
- Channel: Redis
  - 채널은 레디스 캐시를 사용한다.

## Architecture
![pubsub architecture](https://user-images.githubusercontent.com/18645601/79418987-d9bad480-7ff0-11ea-9fe6-cb9c3c4c840f.png)
