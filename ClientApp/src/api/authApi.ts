import axios from 'axios'
import env from 'react-dotenv'
import { LoginRequest, LoginResponse } from './api.types'

export const register = async (data: any) => {
  try {
    const jsonValues = JSON.stringify(data, null, 2)
    const res = await axios(env.API_URL + `/api/auth/register`, {
      method: 'POST',
      headers: {
        'content-type': 'application/json',
      },
      data: jsonValues,
    })
  } catch (error) {
    throw error
  }
}

export const login = async (data: LoginRequest) => {
  try {
    const res = await axios(env.API_URL + `/api/auth/login`, {
      method: 'POST',
      headers: {
        'content-type': 'application/json',
      },
      data,
    })
    return res.data as LoginResponse
  } catch (error) {
    throw error
  }
}
