output "service_url" {
  description = "URL publica do microsservico de upload."
  value       = data.terraform_remote_state.infra.outputs.video_upload_url
}

output "ecs_cluster_name" {
  description = "Cluster ECS compartilhado."
  value       = data.terraform_remote_state.infra.outputs.ecs_cluster_name
}

output "ecs_service_name" {
  description = "Nome do servico ECS de upload."
  value       = data.terraform_remote_state.infra.outputs.ecs_service_names.video_upload
}

output "db_endpoint" {
  description = "Endpoint do banco SQL Server usado pelo upload."
  value       = data.terraform_remote_state.infra.outputs.database_endpoints.video_upload
}

output "db_secret_arn" {
  description = "ARN do secret com DB_CONNECTION_STRING e DB_NAME."
  value       = data.terraform_remote_state.infra.outputs.database_secret_arns.video_upload
}

output "shared_secret_arn" {
  description = "ARN do secret compartilhado com JWT, RabbitMQ e S3."
  value       = data.terraform_remote_state.infra.outputs.shared_secret_arn
}

output "bucket_name" {
  description = "Bucket S3 usado pelos uploads."
  value       = data.terraform_remote_state.infra.outputs.bucket_name
}

output "rabbitmq_host" {
  description = "Hostname interno do RabbitMQ."
  value       = data.terraform_remote_state.infra.outputs.rabbitmq_host
}

output "container_image" {
  description = "Imagem Docker Hub configurada para o video-upload-ms."
  value       = data.terraform_remote_state.infra.outputs.dockerhub_images.video_upload
}
