interface User {
  userName: string
  email: string
}

export interface LoginRequest {
  username: string
  password: string
}
export interface LoginResponse {
  token: string
  expiration: number
  user: User
}

export interface Blog {
  id: number
  name: string
  url: string
}

export interface Subscription {
  id: number
  blog: Blog
  firstSubscribeDate: string
  lastSubscribeDate: string
  lastUnsubscribeDate: string
}

export interface SubscriptionsListResponse {
  list: Subscription[]
}

export interface AddSubscriptionRequest {
  blogUrl: string
}
