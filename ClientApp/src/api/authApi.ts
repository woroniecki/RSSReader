import axios from 'axios'
import env from 'react-dotenv'
import { LoginRequest, LoginResponse } from './api.types'

export const register = async (data: any) => {
  const jsonValues = JSON.stringify(data, null, 2)

  axios(env.API_URL + `/api/auth/register`, {
    method: 'POST',
    headers: {
      'content-type': 'application/json',
    },
    data: jsonValues,
  })
    .then(res => {
      console.log(res)
    })
    .catch(error => {
      console.log(error.response)
    })
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
  } catch (err) {
    throw err
  }
}
