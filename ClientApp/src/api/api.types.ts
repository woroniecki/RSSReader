interface User {
  userName: string
  email: string
}

export interface LoginRequest {
  username: string
  password: string
}

export interface RefreshRequest {
  refreshToken: string
  authToken: string
}

export interface Token {
  token: string
  expires: number
}
export interface LoginResponse {
  authToken: Token
  refreshToken: Token
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

export interface Post {
  id: number
  blogId: number
  title: string
  summary: string
  content: string
  imageUrl: string
  feedUrl: string
  author: string
  publishDate: string
}
