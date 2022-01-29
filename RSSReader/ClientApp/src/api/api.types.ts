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
  role: string
}

export interface Blog {
  id: number
  name: string
  url: string
  description: string
  imageUrl: string
  userData: BlogUserData
}

export interface BlogSearchResponse {
  name: string
  url: string
  imageUrl: string
}

export interface BlogUserData {
  subId: number
  firstSubscribeDate: string
  lastSubscribeDate: string
  lastUnsubscribeDate: string
  groupId: number
  unreadedCount: number
  filterReaded: boolean
}

export interface BlogsListResponse {
  list: Blog[]
}

export interface AddSubscriptionRequest {
  blogUrl: string
  GroupId: number
}

export interface ReadPostRequest {
  blogId: number
  postId: number
}

export interface PatchPostRequest {
  blogId: number
  postId: number
  readed: boolean
  favourite: boolean
}

export interface Post {
  id: number
  blogId: number
  name: string
  summary: string
  content: string
  imageUrl: string
  url: string
  author: string
  publishDate: string
  userData: PostUserData
}

export interface PostUserData {
  readed: boolean
  favourite: boolean
}

export interface Group {
  id: number
  name: string
}

export interface AddGroupRequest {
  name: string
}

export interface RemoveGroupRequest {
  groupId: number
  unsubscribeSubscriptions: boolean
}

export interface PatchSubGroupRequest {
  subId: number
  groupId: number
}

export interface PatchSubscriptionRequest {
  subId: number
  filterReaded: boolean
}
