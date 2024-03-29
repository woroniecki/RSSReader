import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
} from '@reduxjs/toolkit'
import { RootState } from 'store/rootReducer'
import { PatchPostRequest, Post, ReadPostRequest } from '../../api/api.types'
import * as blogApi from '../../api/blogApi'
const ARTICLES = 'articles'

export const articlesAdapter = createEntityAdapter<Post>({
  selectId: sub => sub.id,
})

export const getArticles = createAsyncThunk<
  // Return type of the payload creator
  Post[],
  // First argument to the payload creator
  number,
  {
    rejectValue: string
  }
>(`${ARTICLES}/list`, async (blogid, { rejectWithValue }) => {
  try {
    const res = await blogApi.getPostsList(blogid)
    res.map(el => {
      el.blogId = blogid
    })
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

export const putReadArticle = createAsyncThunk<
  Post,
  ReadPostRequest,
  {
    rejectValue: string
  }
>(`${ARTICLES}/read`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.putReadPost(params.blogId, params.postId)
    res.blogId = params.blogId
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

export const patchPost = createAsyncThunk<
  Post,
  PatchPostRequest,
  {
    rejectValue: string
  }
>(`${ARTICLES}/markReadedFlag`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.patchPost(params)
    res.blogId = params.blogId
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

const articlesSlice = createSlice({
  name: 'articles',
  initialState: articlesAdapter.getInitialState(),
  reducers: {},
  extraReducers: builder => {
    builder
      .addCase(getArticles.fulfilled, (state, { payload }) => {
        articlesAdapter.addMany(state, payload)
      })
      .addCase(putReadArticle.fulfilled, (state, { payload }) => {
        articlesAdapter.upsertOne(state, payload)
      })
      .addCase(patchPost.fulfilled, (state, { payload }) => {
        articlesAdapter.upsertOne(state, payload)
      })
  },
})

export const { actions } = articlesSlice

export default articlesSlice.reducer

export const stateSelector = (state: RootState) => state.articlesReducer

export const {
  selectAll,
  selectById,
  selectIds,
} = articlesAdapter.getSelectors<RootState>(state => state.articlesReducer)
