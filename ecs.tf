
resource "aws_ecs_cluster" "balalaika-ecs-cluster" {
  name = "ecs-cluster-for-balalaika"
}

resource "aws_ecs_task_definition" "balalaika-ecs-task-definition" {
  family                   = "ecs-task-definition-balalaika"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  memory                   = "1024"
  cpu                      = "512"
  task_role_arn            = "arn:aws:iam::686788842590:role/blazorToSqsRole"
  execution_role_arn       = "arn:aws:iam::686788842590:role/ecsTaskExecutionRole"
  container_definitions    = <<EOF
[
  {
    "name": "balalaika-container",
    "image": "686788842590.dkr.ecr.${var.aws_region}.amazonaws.com/${var.ecr_repository}:latest",
    "memory": 1024,
    "cpu": 512,
    "essential": true,
    "entryPoint": ["dotnet", "BlazorApp.dll"],
    "portMappings": [
      {
        "containerPort": 80,
        "hostPort": 80
      }
    ],
    "logConfiguration": {
      "logDriver": "awslogs",
      "options": {
          "awslogs-region" : "${var.aws_region}",
          "awslogs-group" : "${var.ecs_log_group}",
          "awslogs-stream-prefix" : "project"
      }
      }
  }
]
EOF
}

resource "aws_ecs_service" "balalaika-ecs-service" {
  name            = "balalaika-app"
  cluster         = aws_ecs_cluster.balalaika-ecs-cluster.id
  task_definition = aws_ecs_task_definition.balalaika-ecs-task-definition.arn
  launch_type     = "FARGATE"
  network_configuration {
    subnets          = ["subnet-09b5d96bad3291cec"]
    assign_public_ip = true
  }
  desired_count = 1
}


resource "aws_cloudwatch_log_group" "balalaika-log-group" {
  name = var.ecs_log_group
}


resource "aws_sqs_queue" "compute-dlq" {
  name = "compute-dlq"
}

resource "aws_sqs_queue" "compute-queue" {
  name                      = "compute-queue"
  max_message_size          = 2048
  message_retention_seconds = 86400
  receive_wait_time_seconds = 10
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.compute-dlq.arn
    maxReceiveCount     = 3
  })
}

resource "aws_sqs_queue_policy" "compute-queue-policy" {
  queue_url = aws_sqs_queue.compute-queue.id

  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Id": "sqspolicy",
  "Statement": [
    {
      "Sid": "First",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "sqs:*",
      "Resource": "${aws_sqs_queue.compute-queue.arn}"
    }
  ]
}
POLICY
}




resource "aws_iam_role" "lambda_role" {
  name               = "lambda_role"
  assume_role_policy = <<EOF
{
 "Version": "2012-10-17",
 "Statement": [
   {
     "Action": "sts:AssumeRole",
     "Principal": {
       "Service": "lambda.amazonaws.com"
     },
     "Effect": "Allow",
     "Sid": ""
   }
 ]
}
EOF
}

resource "aws_iam_policy" "iam_policy_for_lambda" {

  name        = "aws_iam_policy_for_lambda_role"
  path        = "/"
  description = "AWS IAM Policy for managing aws lambda role"
  policy      = <<EOF
{
 "Version": "2012-10-17",
 "Statement": [
    {
     "Action": [
       "logs:CreateLogGroup",
       "logs:CreateLogStream",
       "logs:PutLogEvents"
     ],
     "Resource": "arn:aws:logs:*:*:*",
     "Effect": "Allow"
    },
    {
      "Effect": "Allow",
      "Action": [
          "s3:*",
          "s3-object-lambda:*"
      ],
      "Resource": "*"
    }
 ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "attach_iam_policy_to_iam_role" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = aws_iam_policy.iam_policy_for_lambda.arn
}


resource "aws_lambda_function" "lambda_function" {
  filename      = "${path.module}/src.zip"
  function_name = "Lambda_Function"
  role          = aws_iam_role.lambda_role.arn
  handler       = "Lambda::Lambda.Function::FunctionHandler"
  runtime       = "dotnet6"
  depends_on    = [aws_iam_role_policy_attachment.attach_iam_policy_to_iam_role]
}

resource "aws_lambda_event_source_mapping" "event_source_mapping" {
  event_source_arn = aws_sqs_queue.compute-queue.arn
  enabled          = true
  function_name    = aws_lambda_function.lambda_function.arn
  batch_size       = 1
}
